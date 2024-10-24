# はじめに

本ドキュメントでは、Windows版の箱庭ドローンシミュレータをARアプリと連携するための方法を説明します。

なお、QUEST3アプリの作成手順については、[こちら](./README-quest3.md)を参照ください。

# 必要な環境セット

- Windows PC
- PS4コントローラ
- QUEST3
- Wi-Fi環境
- [Windows版箱庭ドローンシミュレータ](https://github.com/toppers/hakoniwa-px4sim/blob/main/docs/manual/README.md#python-api%E9%80%A3%E6%90%BA%E7%94%A8%E3%83%9E%E3%83%8B%E3%83%A5%E3%82%A2%E3%83%AB)
  - PS4コントローラで箱庭ドローンを操作できる必要がありますので、事前に準備ください。

# アーキテクチャ

全体アーキテクチャ下図の通りです。

![image](./images/quest3/arch.png)

QUEST3には、箱庭ドローンシミュレータの位置情報を同期して表示するアプリがあります。

このアプリは、起動直後はAR座標の原点を調整するモード(ドローン位置調整アプリ)に入り待機します。

このタイミングで、箱庭用PCから、箱庭ドローン位置調整スクリプト(xr_origin_tuning.py)を実行し、PS4コントローラで原点調整をします。

位置調整完了後、⚪︎ボタンを押下すると、QUEST3アプリは、ドローン位置同期アプリに処理を遷移させ、箱庭ドローンシミュレータ（ARアプリ連携用）と通信しながら、AR空間にドローンを表示させます。


# 事前準備

## 箱庭用PCの環境設定

以下から、AR-Hakoniwa-DEMO-Win.zip をダウンロードしてください。

https://github.com/toppers/hakoniwa-unity-drone-model/releases/tag/v2.2.0


AR-Hakoniwa-DEMO-Win.zip をお好みの場所で解凍してください。

なお、プログラムの実行にはpowershellを使います。

また、ドローン操作および位置合わせは、PS4コントローラを使いますので、事前にUSB接続しておいてください。

### 設定ファイル

QUEST3と箱庭用PCは互いにUDP通信するため、設定ファイルにて、IPアドレスを指定しておく必要があります。

設定ファイルは、`AR-Hakoniwa-DEMO-Win` 直下に、`xr_config.json`が配置されていますので、そこに適切なIPアドレスを設定する必要があります。

書式は以下の通り。

```json
{
    "server_url": "192.168.12.11:54002",
    "client_url": "192.168.12.2:54001",
    "position": [
        0.5,
        0.5,
        0
    ],
    "rotation": [
        0,
        -109.0,
        0
    ]
}
```

- server_url: QUEST3のURLです。
- client_url: 箱庭用PCのURLです。
- position: 初期位置です。`(x, y, z)` の並び順で、Unity座標系です。単位は`m(メートル)`です。
- rotation: 初期角度です。`(x, y, z)` の並び順で、Unity座標系です。単位は`degree(度)`です。

事前に、pingコマンドなどで、PCからQUEST3に物理的に通信できることを確認してください。


## QUEST3の環境設定

以下から、model.apk をダウンロードしてください。

https://github.com/toppers/hakoniwa-unity-drone-model/releases/tag/v2.2.0


model.apkをQUEST3にインストールしてください。

参考：https://vr-peak.blog/how-to-install-apps-outside-the-meta-store-on-meta-quest-3/


# 操作手順

1. [QUEST3でARアプリを起動する](#QUEST3でARアプリを起動する)
2. [箱庭ドローン位置調整をする](#箱庭ドローン位置調整をする)
3. [ドローン機体操作アプリの起動](https://github.com/toppers/hakoniwa-px4sim/blob/main/docs/manual/windows_hakowin_installer.md#534-%E3%83%89%E3%83%AD%E3%83%BC%E3%83%B3%E6%A9%9F%E4%BD%93%E6%93%8D%E4%BD%9C%E3%82%A2%E3%83%97%E3%83%AA%E3%81%AE%E8%B5%B7%E5%8B%95)
5. [AR連携用箱庭Unityアプリの起動](#AR連携用箱庭Unityアプリの起動)
6. PS4コントローラで箱庭ドローンを操作してください。

## QUEST3でARアプリを起動する

QUEST3を装着して、model.apk を起動してください。
アプリが正常に起動すると、Unityのロゴが表示された後に、パススルーモードになります。
このタイミングでは、ドローンは何も表示されません。次の手順に従ってください。

## 箱庭ドローン位置調整をする

まず、powershell で、`AR-Hakoniwa-DEMO-Win/utils` へ移動してください。

箱庭ドローン位置調整スクリプトの実行方法は以下の通りです。

```bash
python xr_origin_tuning.py --input joystick ../xr_config.json <QUEST3のIPアドレス>:38528
```

xr_origin_tuning_wing.pyを起動すると、OKのメッセージが出れば成功です。

ERRORの場合は通信できてませんので、ネットワーク環境などのチェックが必要です。

OKメッセージが出ると、箱庭ドローンがQUEST3のAR空間に表せますので、PS4コントローラで位置合わせを行います。
PS4コントローラのスティック操作で位置が変わりますので、所望の場所にドローンを移動させたら丸ボタンを押下してください。

すると、xr_origin_tuning_win.pyが終了します。


## AR連携用箱庭Unityアプリの起動

Windows エクスプローラで、AR-Hakoniwa-DEMO-Win を開き、`model.exe` をダブルクリックしてください。

Unityアプリが起動すると、QUEST3内のアプリケーションとの通信が始まります。
この時に、通信がうまく行っていれば、PC上で自分の顔の向きに合わせてアバターが動きます。



