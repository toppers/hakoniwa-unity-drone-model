class PIDController:
    def __init__(self, kp, ki, kd):
        self.kp = kp  # 比例ゲイン
        self.ki = ki  # 積分ゲイン
        self.kd = kd  # 微分ゲイン
        self.prev_error = [ 0.0, 0.0, 0.0 ]  # 前回のエラー
        self.integral = [ 0.0, 0.0, 0.0 ]  # 積分項

    def control(self, target_angles, current_angles, dt):
        # 姿勢角の誤差を計算
        error = [
            target_angle - current_angle
            for target_angle, current_angle in zip(target_angles, current_angles)
        ]

        # PID制御計算
        control_signal = [
            self.kp * e +
            self.ki * i +
            self.kd * (e - prev_e) / dt
            for e, i, prev_e in zip(error, self.integral, self.prev_error)
        ]

        # 制御信号を適用（例: ドローンのパラメータ設定など）

        # エラーと積分項を更新
        self.prev_error = error
        self.integral = [
            i + e * dt
            for i, e in zip(self.integral, error)
        ]

        return control_signal
    