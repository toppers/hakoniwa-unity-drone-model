# はじめに

本機能は、[XCharts](https://github.com/XCharts-Team/XCharts) の基本機能を利用して、箱庭ドローンの内部データをグラフ化するものです。

現時点では、機体内部温度をビジュアライズできるようにしています。

# インストール手順

1. Unityのパッケージマネージャの "Add package from git URL.."を選択し、以下を入力します。
   - https://github.com/XCharts-Team/XCharts.git
2. third-party/XCharts のライセンス条項を[こちら](LICENSE.md)で確認し、適切にコピーして`plugin/plugin-srcs/Assets/Third-party/`に配置してください。
3. DroneDataVisualizerプレハブを `Hakoniwa/GUI/Simulation`配下に配置してください。

# ライセンス

本機能が利用している基本機能は、以下のみであり、"extended charts" は利用していません。そのため、追加のライセンスの購入は不要と判断しています。
- LineChart

## XChartsのライセンス条項
- [MIT License](LICENSE.md)
- Free commercial, secondary development
- The extended charts and advanced features sections require a separate purchase license
