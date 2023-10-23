using Hakoniwa.PluggableAsset.Assets.Robot;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Parts
{
    struct HilStateQuaternion
    {
        public ulong time_usec;
        public float[] attitude_quaternion;
        public float rollspeed;
        public float pitchspeed;
        public float yawspeed;
        public int lat;
        public int lon;
        public int alt;
        public short vx; // Using C#'s short as equivalent to int16
        public short vy;
        public short vz;
        public ushort ind_airspeed; // Using C#'s ushort as equivalent to uint16
        public ushort true_airspeed;
        public short xacc;
        public short yacc;
        public short zacc;
    }

    public class Px4Sensor : MonoBehaviour, IRobotPartsSensor, IRobotPartsConfig
    {
        private HilStateQuaternion hil_state_quaternion = new HilStateQuaternion
        {
            time_usec = 0,
            attitude_quaternion = new float[4] { 1f, 0f, 0f, 0f },
            rollspeed = 0f,
            pitchspeed = 0f,
            yawspeed = 0f,
            lat = 0,
            lon = 0,
            alt = 0,
            vx = 0,
            vy = 0,
            vz = 0,
            ind_airspeed = 0,
            true_airspeed = 0,
            xacc = 0,
            yacc = 0,
            zacc = 0
        };
        [Header("Reference Geographical Location")]
        public float referenceLatitude = 35.6895f; // Tokyo latitude as an example
        public float referenceLongitude = 139.6917f; // Tokyo longitude as an example
        public float referenceAltitude = 0f; // Sea level as an example

        private float deltaTime;
        private Vector3 prev_velocity = Vector3.zero;
        private Rigidbody my_rigidbody;
        private Vector3 prev_angle = Vector3.zero;
        private Vector3 delta_angle = Vector3.zero;

        private GameObject root;
        private GameObject sensor;
        private PduIoConnector pdu_io;
        private IPduWriter[] pdu_writer = new IPduWriter[3];
        private string root_name;

        public string[] topic_type = {
            "hako_mavlink_msgs/HakoHilSensor",
            "hako_mavlink_msgs/HakoHilStateQuaternion",
            "hako_mavlink_msgs/HakoHilGps"
        };
        public int update_cycle = 1;
        public string[] topic_name = {
            "hil_sensor",
            "hil_state_quaternion",
            "hil_gps"
        };
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[3];
            configs[0] = new RoboPartsConfigData();
            configs[0].io_dir = IoDir.WRITE;
            configs[0].io_method = this.io_method;
            configs[0].value.org_name = this.topic_name[0];
            configs[0].value.type = this.topic_type[0];
            configs[0].value.class_name = ConstantValues.pdu_writer_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[0].value.pdu_size = 72;
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();

            configs[1] = new RoboPartsConfigData();
            configs[1].io_dir = IoDir.WRITE;
            configs[1].io_method = this.io_method;
            configs[1].value.org_name = this.topic_name[1];
            configs[1].value.type = this.topic_type[1];
            configs[1].value.class_name = ConstantValues.pdu_writer_class;
            configs[1].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[1].value.pdu_size = 64;
            configs[1].value.write_cycle = this.update_cycle;
            configs[1].value.method_type = this.comm_method.ToString();

            configs[2] = new RoboPartsConfigData();
            configs[2].io_dir = IoDir.WRITE;
            configs[2].io_method = this.io_method;
            configs[2].value.org_name = this.topic_name[2];
            configs[2].value.type = this.topic_type[2];
            configs[2].value.class_name = ConstantValues.pdu_writer_class;
            configs[2].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
            configs[2].value.pdu_size = 40;
            configs[2].value.write_cycle = this.update_cycle;
            configs[2].value.method_type = this.comm_method.ToString();
            return configs;
        }

        public RosTopicMessageConfig[] getRosConfig()
        {
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[3];
            cfg[0] = new RosTopicMessageConfig();
            cfg[0].topic_message_name = this.topic_name[0];
            cfg[0].topic_type_name = this.topic_type[0];
            cfg[0].sub = false;
            cfg[0].pub_option = new RostopicPublisherOption();
            cfg[0].pub_option.cycle_scale = this.update_cycle;
            cfg[0].pub_option.latch = false;
            cfg[0].pub_option.queue_size = 1;

            cfg[1] = new RosTopicMessageConfig();
            cfg[1].topic_message_name = this.topic_name[1];
            cfg[1].topic_type_name = this.topic_type[1];
            cfg[1].sub = false;
            cfg[1].pub_option = new RostopicPublisherOption();
            cfg[1].pub_option.cycle_scale = this.update_cycle;
            cfg[1].pub_option.latch = false;
            cfg[1].pub_option.queue_size = 1;

            cfg[2] = new RosTopicMessageConfig();
            cfg[2].topic_message_name = this.topic_name[2];
            cfg[2].topic_type_name = this.topic_type[2];
            cfg[2].sub = false;
            cfg[2].pub_option = new RostopicPublisherOption();
            cfg[2].pub_option.cycle_scale = this.update_cycle;
            cfg[2].pub_option.latch = false;
            cfg[2].pub_option.queue_size = 1;
            return cfg;
        }

        private Vector3 lastVelocity;
        public void Initialize(System.Object obj)
        {
            GameObject tmp = null;
            try
            {
                tmp = obj as GameObject;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Initialize error: " + e.Message);
                return;
            }

            if (this.root == null)
            {
                this.root = tmp;
                this.root_name = string.Copy(this.root.transform.name);
                this.pdu_io = PduIoConnector.Get(root_name);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + root_name);
                }
                var pdu_writer_name = root_name + "_" + this.topic_name[0] + "Pdu";
                this.pdu_writer[0] = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer == null)
                {
                    throw new ArgumentException("can not found pdu_writer:" + pdu_writer_name);
                }
                pdu_writer_name = root_name + "_" + this.topic_name[1] + "Pdu";
                this.pdu_writer[1] = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer == null)
                {
                    throw new ArgumentException("can not found pdu_writer:" + pdu_writer_name);
                }
                pdu_writer_name = root_name + "_" + this.topic_name[2] + "Pdu";
                this.pdu_writer[2] = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer == null)
                {
                    throw new ArgumentException("can not found pdu_writer:" + pdu_writer_name);
                }

                this.sensor = this.gameObject;
                this.my_rigidbody = this.GetComponentInChildren<Rigidbody>();
                if (this.my_rigidbody == null)
                {
                    throw new ArgumentException("IMUSensor can not find RigidBody: " + this.sensor.name);
                }
                this.deltaTime = Time.fixedDeltaTime;
                lastVelocity = my_rigidbody.velocity;

            }
        }

        public bool isAttachedSpecificController()
        {
            return false;
        }

        public void UpdateSensorValues()
        {
            //MUST FIRST STATE QUATERNION!
            UpdateHilStateQuaternion(this.pdu_writer[1].GetWriteOps().Ref(null));
            //NEXT HERE
            UpdateHilSensor(this.pdu_writer[0].GetWriteOps().Ref(null));
            UpdateHilGps(this.pdu_writer[2].GetWriteOps().Ref(null));
        }



        // CalculateAltitude, CalculateLatitude, and CalculateLongitude functions remain the same as before



        private const int AVERAGE_COUNT = 1;
        private List<Vector3> accelerationSamples = new List<Vector3>();

        Vector3 CalculateAverage(List<Vector3> samples)
        {
            Vector3 sum = Vector3.zero;
            foreach (var sample in samples)
            {
                sum += sample;
            }
            return sum / samples.Count;
        }

        private void UpdateHilStateQuaternion(Pdu pdu)
        {
            Vector3 currentVelocity = my_rigidbody.velocity;

            // Time (you might want to get the actual timestamp value)
            ulong time_usec = 0;

            // Angular velocities in Unity's coordinate system
            Vector3 unityAngularVelocity = my_rigidbody.angularVelocity;

            // Convert to MAVLink's coordinate system (NED)
            float rollspeed = unityAngularVelocity.z;  // Unity's z becomes MAVLink's roll
            float pitchspeed = -unityAngularVelocity.x; // Unity's x becomes MAVLink's -pitch
            float yawspeed = unityAngularVelocity.y;    // Unity's y becomes MAVLink's yaw

            // Linear velocities in Unity's coordinate system
            Vector3 unityVelocity = my_rigidbody.velocity;

            // Convert to MAVLink's coordinate system (NED)
            short vx = (short)(unityVelocity.z * 100);  // Unity's z (前方) becomes MAVLink's x
            short vy = (short)(-unityVelocity.x * 100); // Unity's x (右方) becomes MAVLink's -y (左方)
            short vz = (short)(unityVelocity.y * 100);  // Unity's y (上方) becomes MAVLink's z

            Vector3 unityAcceleration = (currentVelocity - lastVelocity) / deltaTime;
            //gravity element
            unityAcceleration += transform.InverseTransformDirection(Physics.gravity);
            //Debug.Log("lastVelocity = " + currentVelocity);
            //Debug.Log("currentVelocity = " + currentVelocity);
            // 新しいサンプルをリストに追加
            accelerationSamples.Add(unityAcceleration);

            // サンプルの数が10を超えた場合、古いサンプルを削除
            if (accelerationSamples.Count > AVERAGE_COUNT)
            {
                accelerationSamples.RemoveAt(0);
            }

            // 平均を計算
            Vector3 averageAcceleration = CalculateAverage(accelerationSamples);

            // MAVLinkの座標系に変換
            short xacc = (short)(averageAcceleration.z * 1000);  // Unity's z becomes MAVLink's x
            short yacc = (short)(-averageAcceleration.x * 1000); // Unity's x becomes MAVLink's -y
            short zacc = (short)(averageAcceleration.y * 1000);  // Unity's y becomes MAVLink's z

            lastVelocity = currentVelocity;

            // Populate the struct
            hil_state_quaternion.time_usec = time_usec;
            hil_state_quaternion.attitude_quaternion = new float[4] {
                //order: w, x, y, z
                my_rigidbody.rotation.w,
                my_rigidbody.rotation.z,
                -my_rigidbody.rotation.x,
                my_rigidbody.rotation.y
            };
            hil_state_quaternion.rollspeed = rollspeed;
            hil_state_quaternion.pitchspeed = pitchspeed;
            hil_state_quaternion.yawspeed = yawspeed;
            hil_state_quaternion.vx = vx;
            hil_state_quaternion.vy = vy;
            hil_state_quaternion.vz = vz;
            hil_state_quaternion.xacc = xacc;
            hil_state_quaternion.yacc = yacc;
            hil_state_quaternion.zacc = zacc;

            // Set the data to the PDU using the struct's fields
            pdu.SetData("time_usec", hil_state_quaternion.time_usec);
            pdu.SetData("attitude_quaternion", hil_state_quaternion.attitude_quaternion);
            pdu.SetData("rollspeed", hil_state_quaternion.rollspeed);
            pdu.SetData("pitchspeed", hil_state_quaternion.pitchspeed);
            pdu.SetData("yawspeed", hil_state_quaternion.yawspeed);
            pdu.SetData("vx", hil_state_quaternion.vx);
            pdu.SetData("vy", hil_state_quaternion.vy);
            pdu.SetData("vz", hil_state_quaternion.vz);
            pdu.SetData("xacc", hil_state_quaternion.xacc);
            pdu.SetData("yacc", hil_state_quaternion.yacc);
            pdu.SetData("zacc", hil_state_quaternion.zacc);

            pdu.SetData("ind_airspeed", hil_state_quaternion.ind_airspeed);
            pdu.SetData("true_airspeed", hil_state_quaternion.true_airspeed);

            //GPS
            Vector3 dronePosition = my_rigidbody.position;
            int lat = CalculateLatitude(dronePosition, referenceLatitude);
            int lon = CalculateLongitude(dronePosition, referenceLongitude);
            int alt = CalculateAltitude(dronePosition, referenceAltitude);

            pdu.SetData("lat", lat);
            pdu.SetData("lon", lon);
            pdu.SetData("alt", alt);

            //Debug.Log("lat=" + lat  + " lon= " + lon + " alt=" + alt);
        }
        /*
         * Unityの位置情報を実際の緯度、経度、高度に変換するための実装は、使用する地球モデルやシミュレーションのスケールに依存します。
         * しかし、以下の方法は、Unityのシミュレーション内での小規模な移動を考慮して、単純な線形変換を利用した例を示します。
         *
         * 緯度の1度あたりの距離は約111kmです。
         * 経度の1度あたりの距離は、緯度に依存しますが、赤道での距離は約111kmであり、極点に近づくにつれてこの距離は短くなります。
         * Unityの位置（メートル単位）を上記の情報を元に緯度、経度に変換します。
         */
        /*
         * Unityの標準的な座標系では、以下のようになっています：
         * 
         *  X軸：左右
         *  Y軸：上下
         *  Z軸：前後
         *  
         * 一方、地理的な座標では：
         * 
         *  Latitude（緯度）: 北と南
         *  Longitude（経度）: 東と西
         *  Altitude（高度）: 海抜の高さ
         *  
         * したがって、Unityの座標を地理的な座標に変換する場合、以下のように考慮すべきです：
         * 
         *  X軸 → 経度 (Longitude)
         *  Y軸 → 高度 (Altitude)
         *  Z軸 → 緯度 (Latitude)
         */
        private int CalculateAltitude(Vector3 dronePosition, float referenceAltitude)
        {
            // Unity's Y axis represents altitude.
            // Assuming dronePosition.y is in meters and we need to return altitude in millimeters.
            int altitude = (int)((dronePosition.y + referenceAltitude) * 1000);
            return altitude;
        }

        private int CalculateLongitude(Vector3 dronePosition, float referenceLongitude)
        {
            // Convert Unity position (in meters) to change in longitude based on reference.
            float deltaLongitude = dronePosition.x / 111000f; // Unity's X axis represents East-West direction.
            int longitude = (int)((referenceLongitude + deltaLongitude) * 1e7); // Convert to 1e7 format used by MAVLink
            return longitude;
        }

        private int CalculateLatitude(Vector3 dronePosition, float referenceLatitude)
        {
            // Convert Unity position (in meters) to change in latitude based on reference.
            float deltaLatitude = dronePosition.z / 111000f; // Unity's Z axis represents North-South direction.
            int latitude = (int)((referenceLatitude + deltaLatitude) * 1e7); // Convert to 1e7 format used by MAVLink
            return latitude;
        }
        // 東京の地磁気の北方向を示すベクトル
        // ここでは強度も0.5ガウスで考慮しています
        private Vector3 TOKYO_MAGNETIC_NORTH = new Vector3(0, 0.5f, 0);

        private Vector3 CalcMAVLinkMagnet()
        {
            // センサーの現在の回転に基づいて磁場の方向と強度を調整
            Vector3 adjustedMagneticNorth = sensor.transform.rotation * TOKYO_MAGNETIC_NORTH;

            return adjustedMagneticNorth;
        }

        private void UpdateHilSensor(Pdu pdu)
        {
            // Use values from hil_state_quaternion or default/dummy values
            ulong time_usec = 0;

            float xacc = hil_state_quaternion.xacc / 1000.0f;  // Convert from mm/s^2 to m/s^2
            float yacc = hil_state_quaternion.yacc / 1000.0f;
            float zacc = hil_state_quaternion.zacc / 1000.0f;

            float xgyro = hil_state_quaternion.rollspeed;  // Assuming radian/sec
            float ygyro = hil_state_quaternion.yawspeed;
            float zgyro = hil_state_quaternion.pitchspeed;

            var mag = CalcMAVLinkMagnet();
            float xmag = mag.x;
            float ymag = mag.y;
            float zmag = mag.z;
            float abs_pressure = 1013.25f;  // Standard atmospheric pressure at sea level
            float diff_pressure = 0.0f;  // Differential pressure (used for airspeed calculation)
            float pressure_alt = 0.0f;  // Pressure altitude
            float temperature = 20.0f;  // Assume 20 degrees Celsius by default

            uint fields_updated = 0x1FFF;  // Bitmask indicating which fields are valid (assuming all fields are updated for simplicity)
            byte id = 0;  // Sensor instance ID (use default 0)

            // Set the data to the PDU
            pdu.SetData("time_usec", time_usec);
            pdu.SetData("xacc", xacc);
            pdu.SetData("yacc", yacc);
            pdu.SetData("zacc", zacc);
            pdu.SetData("xgyro", xgyro);
            pdu.SetData("ygyro", ygyro);
            pdu.SetData("zgyro", zgyro);
            pdu.SetData("xmag", xmag);
            pdu.SetData("ymag", ymag);
            pdu.SetData("zmag", zmag);
            pdu.SetData("abs_pressure", abs_pressure);
            pdu.SetData("diff_pressure", diff_pressure);
            pdu.SetData("pressure_alt", pressure_alt);
            pdu.SetData("temperature", temperature);
            pdu.SetData("fields_updated", fields_updated);
            pdu.SetData("id", id);
        }
        private void UpdateHilGps(Pdu pdu)
        {
            // Time (you might want to get the actual timestamp value)
            ulong time_usec = 0;

            // GPS Fix Type
            byte fix_type = 3; // 3D Fix

            // GPS position (latitude, longitude, and altitude)
            Vector3 dronePosition = my_rigidbody.position;
            int lat = CalculateLatitude(dronePosition, referenceLatitude);
            int lon = CalculateLongitude(dronePosition, referenceLongitude);
            int alt = CalculateAltitude(dronePosition, referenceAltitude);

            // GPS dilution of position (horizontal and vertical)
            ushort eph = 100; // Example value (unitless * 100)
            ushort epv = 100; // Example value (unitless * 100)

            // GPS ground speed
            ushort vel = (ushort)(my_rigidbody.velocity.magnitude * 100); // Convert to cm/s

            // GPS velocity in NED frame
            short vn = (short)(my_rigidbody.velocity.z * 100); // Convert to cm/s (North)
            short ve = (short)(my_rigidbody.velocity.x * 100); // Convert to cm/s (East)
            short vd = (short)(-my_rigidbody.velocity.y * 100); // Convert to cm/s (Down)

            // Course over ground
            ushort cog = 0; // Example value (0 degrees)

            // Number of satellites visible
            byte satellites_visible = 10; // Example value

            byte id = 0;
            byte yaw = 0;

            // Set the data to the PDU
            pdu.SetData("time_usec", time_usec);
            pdu.SetData("fix_type", fix_type);
            pdu.SetData("lat", lat);
            pdu.SetData("lon", lon);
            pdu.SetData("alt", alt);
            pdu.SetData("eph", eph);
            pdu.SetData("epv", epv);
            pdu.SetData("vel", vel);
            pdu.SetData("vn", vn);
            pdu.SetData("ve", ve);
            pdu.SetData("vd", vd);
            pdu.SetData("cog", cog);
            pdu.SetData("satellites_visible", satellites_visible);

            pdu.SetData("id", id);
            pdu.SetData("yaw", yaw);
        }
    }
}
