ここでは、Unityエディタを利用したネイティブWindows版の箱庭ドローンシミュレータのインストールおよびシミュレーション実行手順を説明します。

# 目次

- [利用するソフトウェア](#利用するソフトウェア)
- [ネイティブWindows版の箱庭ドローンシミュレータのインストール](#ネイティブwindows版の箱庭ドローンシミュレータのインストール)
- [Unity エディタのセットアップ手順](#unity-エディタのセットアップ手順)
- [Pythonアプリとの連携方法](#pythonアプリとの連携方法)
- [参考：Unity アプリケーションの作成手順](#参考unity-アプリケーションの作成手順)

# 利用するソフトウェア

- [Git for Windows](https://gitforwindows.org/)
- Unity Hub
- Unity
- RAM Disk
- Python

`Git for Windows` のインストールおよび利用方法については、公式サイトを参照ください。

`Git for WIndows`以外のインストール方法は、[箱庭ドローンシミュレータ_事前インストール編_windows版](https://www.jasa.or.jp/dl/tech/drone_simulator_pre-installation_version.pdf)を参照ください。


# ネイティブWindows版の箱庭ドローンシミュレータのインストール

[箱庭ドローンシミュレータ_操作編](https://www.jasa.or.jp/dl/tech/simulator_operation_edition.pdf)の以下を実施して、`hakoniwa-px4-win` をインストールしてください。

- 箱庭ドローンシミュレータの導⼊
- 箱庭ドローンシミュレータ⽤のコンフィグパスの設定

# Unity エディタのセットアップ手順

## Git クローン
Git ツールを利用して、`https://github.com/toppers/hakoniwa-unity-drone-model.git` をクローンしてください。

ここで、クローンするフォルダは、Eドライブの project というフォルダを前提として解説を進めます。

![image](images/GitGui.png)

Git Gui で、`Clone Existing Repository` をクリックします。


![image](images/GitClone.png)

* Source Location: https://github.com/toppers/hakoniwa-unity-drone-model.git
* Target Directory: E:/project/hakoniwa-unity-drone-model

上記のように設定して、`Clone` をクリックします。

成功すると、下図のようにGit管理用の画面に変わります。

![image](images/GitCloneSuccess.png)

## 箱庭のインストール

Windows エクスプローラを開き、クローンしたフォルダを表示してください。

![image](images/WindowsExploreForInstall.png)

`hakoniwa-unity-drone-model` フォルダの下に、`install.bat` というバッチファイルがありますので、ダブルクリックすると黒い画面が出力されて、箱庭のインストールが実行されます。

成功すると、下図のように、`Process completed successfully`と出力されますので、黒い画面を閉じてください。

![image](images/InstallSuccess.png)


## Unityエディタのセットアップ

Unity Hubを起動してください。

![image](images/UnityHub.png)

`Add` をクリックし、`E:/project/hakoniwa-unity-drone-model/plugin/plugin-srcs` を選択してください。

![image](images/UnityHubAddProject.png)

そのまま選択したプロジェクトを開きましょう。この際、以下のようにバージョンが違う旨のダイアログが出力される場合は、利用されているUnityバージョンを選択して開いてください。自動でバーション変換が行われます。

![image](images/UnityVersionSelect.png)

以下のダイアログが出ますが、`Continue` してください。

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/e1fbc477-4edc-4e39-ab15-ccd6f0707f33)


次に、以下のダイアログが出ますので、`Ignore` してください。

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/7c03ae41-f988-44cb-9ac1-2263507d254d)


成功するとこうなります。

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/50398cfa-f6fc-4eef-9679-5442bbd9de76)

起動直後の状態ですと、コンソール上にたくさんエラーが出ていますので、以下のリンク先を参照して、対応してください。

* [gRPC のライブラリ利用箇所がエラー出力している](https://github.com/toppers/hakoniwa-document/blob/main/troubleshooting/unity/README.md#grpc-%E3%81%AE%E3%83%A9%E3%82%A4%E3%83%96%E3%83%A9%E3%83%AA%E5%88%A9%E7%94%A8%E7%AE%87%E6%89%80%E3%81%8C%E3%82%A8%E3%83%A9%E3%83%BC%E5%87%BA%E5%8A%9B%E3%81%97%E3%81%A6%E3%81%84%E3%82%8B)


## 箱庭のセットアップ

プロジェクトビューの`Scenes`を選択して、`ApiDemo`をダブルクリックしてください。成功するとこうなります。

![image](images/UnityScne.png)

Unityエディタのメニュー `Window/Hakoniwa/Generate` をクリックしてください。

成功すると、コンソール上にエラーログ出力がなく、下図のように json のログが出力されています。

![スクリーンショット 2023-08-28 8 00 16](https://github.com/toppers/hakoniwa-unity-picomodel/assets/164193/6fa55a56-1693-4728-b0ef-091e10fb4b22)


# Pythonアプリとの連携方法

Pythonアプリで箱庭ドローンを操作するためには、先述した `hakoniwa-px4-win` のインストールが必要です。

`hakoniwa-px4-win` のフォルダ内には以下の重要なファイルがあります。

- hakoniwa/apps/sample.py
  - 箱庭ドローンを制御するためのPythonプログラムです。お好みのPythonエディタを使用して、ご自分のプログラムを作成できます。
- hakoniwa/apps/run-sample.bat
  - sample.pyを起動するバッチファイルです。sample.pyを実行するには、このバッチファイルをダブルクリックします。
- hakoniwa/bin/run-api2.bat
  - 箱庭ドローンシミュレータを起動するバッチファイルです。

Pythonアプリを実行して箱庭ドローンシミュレーションを実行する方法は以下の通りです。

## 事前準備

シミュレーション実行するために、事前に以下を実施ください。

1. Windowsエクスプローラで、`hakoniwa-px4-win/hakoniwa/bin/` を開きます。
2. 別のWindowsエクスプローラで、`hakoniwa/apps` を開きます。
3. 先述した手順でセットアップ完了したUnityエディタを開きます。


## シミュレーション実行方法


`hakoniwa/bin/run-api2.bat`をダブルクリックします。

成功すると、下図の黒画面が出力されます。

![image](images/run-api2.png)

次に、Unityエディタのシミュレーション開始ボタンをクリックします。

成功すると、下図のようにゲームビューに切り替わりますので `START`ボタンをクリックします。

![iamge](images/UnitySim.png)

最後に、`hakoniwa/apps/run-sample.bat` をダブルクリックすると、以下の黒画面が出力され、箱庭ドローンが動き出します。

![iamge](images/PythonExec.png)

## シミュレーション停止方法

シミュレーションを終了するには、以下の手順で停止してください。

1. Unityエディタのシミュレーション停止ボタンを押下します。
2. `hakoniwa/apps/run-sample.bat`の黒画面を閉じます。
3. `hakoniwa/bin/run-api2.bat`の黒画面を閉じます。

# 参考：Unity アプリケーションの作成手順

Unityエディタ上で `ApiDemo` シーンを編集して、Unityアプリケーション化したい場合は、以下のサイトに手順があります。


* [Unityアプリケーションの作成方法](https://github.com/toppers/hakoniwa-px4sim/tree/main/tools/win#unity%E3%82%A2%E3%83%97%E3%83%AA%E3%82%B1%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3%E3%81%AE%E4%BD%9C%E6%88%90)
