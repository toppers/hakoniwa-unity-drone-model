# サポート環境

## Mac環境

- [ ] Intel系 Mac
- [x] Arm系 Mac

## Windows 環境

- [x] Windows 10/11


# 前提とする環境

- Homebrew(Mac版のみ)
- Unity Hub
- Unity（各CPUアーキテクチャに対応したもの）
  - 2022.3.5f1 以降
- Python 3.10（pyenvでインストールされたものMac版のみ。）

# インストール方法

本リポジトリを以下のようにクローンします。

```
git clone --recursive https://github.com/toppers/hakoniwa-unity-drone-model.git
```

クローンが終わったら、以下のようにディレクトリ移動します。

```
cd hakoniwa-unity-drone-model/
```

そして、必要な Unity モジュール類をインストールします。

```
bash install.bash 
```


# Unity起動

この状態で Unity Hub で当該プロジェクトを開きましょう。

注意：Unityエディタは、当該CPUアーキテクチャに対応したものをインストールしてご利用ください。

対象フォルダ：hakoniwa-unity-drone-model\plugin\plugin-srcs

Unityのバージョン違いに起因するメッセージ（"Opening Project in Non-Matching Editor Installation"）が出る場合は、「Continue」として問題ありません。

以下のダイアログが出ますが、`Continue` してください。

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/e1fbc477-4edc-4e39-ab15-ccd6f0707f33)


次に、以下のダイアログが出ますので、`Ignore` してください。

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/7c03ae41-f988-44cb-9ac1-2263507d254d)


成功するとこうなります。

![image](https://github.com/toppers/hakoniwa-unity-drone-model/assets/164193/50398cfa-f6fc-4eef-9679-5442bbd9de76)

起動直後の状態ですと、コンソール上にたくさんエラーが出ています。原因は以下の２点です。
リンク先を参照して、順番に対応してください。

* [Newtonsoft.Json が不足している](https://github.com/toppers/hakoniwa-document/tree/main/troubleshooting/unity#unity%E8%B5%B7%E5%8B%95%E6%99%82%E3%81%ABnewtonsoftjson%E3%81%8C%E3%81%AA%E3%81%84%E3%81%A8%E3%81%84%E3%81%86%E3%82%A8%E3%83%A9%E3%83%BC%E3%81%8C%E5%87%BA%E3%82%8B)
* [gRPC のライブラリ利用箇所がエラー出力している](https://github.com/toppers/hakoniwa-document/blob/main/troubleshooting/unity/README.md#grpc-%E3%81%AE%E3%83%A9%E3%82%A4%E3%83%96%E3%83%A9%E3%83%AA%E5%88%A9%E7%94%A8%E7%AE%87%E6%89%80%E3%81%8C%E3%82%A8%E3%83%A9%E3%83%BC%E5%87%BA%E5%8A%9B%E3%81%97%E3%81%A6%E3%81%84%E3%82%8B)(Mac版のみ)


# シミュレーション環境の準備

下図のように、Unity のシーンをダブルクリックします。

![スクリーンショット 2023-08-28 7 58 37](https://github.com/toppers/hakoniwa-unity-picomodel/assets/164193/d60d2bb2-ee77-441c-aed8-a07a0ada17f4)


そして、`Window` -> `Hakoniwa` -> `Generate` をクリックします。

![スクリーンショット 2023-08-28 7 59 45](https://github.com/toppers/hakoniwa-unity-picomodel/assets/164193/85ab96b7-fd8b-4547-a4a3-c386d0a35813)


成功すると、コンソール上にエラーログ出力がなく、下図のように json のログが出力されています。

![スクリーンショット 2023-08-28 8 00 16](https://github.com/toppers/hakoniwa-unity-picomodel/assets/164193/6fa55a56-1693-4728-b0ef-091e10fb4b22)
