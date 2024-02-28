using System;
using System.Collections;
using System.Collections.Generic;
using Hakoniwa.PluggableAsset.Assets.Environment;
using UnityEngine;
using XCharts.Runtime;

namespace Hakoniwa.GUI.Visualizer
{
    public struct HakoDataContainer
    {
        public int index;
        public string name;
        public IRobotProperty prop;
    }
    public class HakoDataVisualizer : MonoBehaviour
    {
        // Start is called before the first frame update
        public LineChart chart;
        public string title_name = "Temperature";
        public float y_max = 60;
        public float y_min = -40;
        public float interval_sec = 1;
        private List<HakoDataContainer> list = new List<HakoDataContainer>();

        private void InitChart()
        {
            var root = GameObject.Find("Robot");
            if (root == null)
            {
                throw new NotSupportedException("ERROR: Robot is not found..");
            }
            chart.RemoveData();
            int index = 0;
            Debug.Log("Visualizer childcount=" + root.transform.childCount);
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform child = root.transform.GetChild(i);
                Debug.Log("Visualizer: " + child.name);
                var p = child.GetComponentInChildren<IRobotProperty>();
                if (p != null)
                {
                    var container = new HakoDataContainer();
                    container.name = child.name;
                    container.index = index;
                    container.prop = p;
                    list.Add(container);
                    Debug.Log("Visualizer: Entry " + child.name);
                    index++;
                    chart.AddSerie<Line>(container.name);
                }
            }
            
            var tooltip = chart.EnsureChartComponent<Tooltip>();
            tooltip.show = true;

            var legend = chart.EnsureChartComponent<Legend>();
            legend.show = true;

            var xAxis = chart.EnsureChartComponent<XAxis>();
            xAxis.splitNumber = 10;
            xAxis.boundaryGap = true;
            xAxis.type = Axis.AxisType.Time;

            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.type = Axis.AxisType.Value;
            yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
            yAxis.max = y_max;
            yAxis.min = y_min;
        }
        void Start()
        {
            Debug.Log("CHART START");
            if (chart == null)
            {
                Debug.LogWarning("LineChart component is not found.");
                return;
            }
            var title = chart.EnsureChartComponent<Title>();
            title.text = title_name;
            InitChart();

        }
        private float elapsed_sec = 0;
        void Update()
        {
            elapsed_sec += Time.deltaTime;
            if (elapsed_sec < interval_sec)
            {
                //nothing to do
            }
            else
            {
                foreach (var container in list) {
                    double newData = container.prop.GetTemperature();
                    chart.AddData(container.index, newData);
                    Debug.Log("Temp=" + newData);
                }
                elapsed_sec = 0;
            }
        }
    }

}

