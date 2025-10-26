# 最初に
- 1: このdllはUnity用に作成されています。使用する環境を間違えないようにお願いします。
- 2: dllを作る際に参照したUnity Editorのバージョンは、6000.0.58f2(LTS)です。
※セキュリティ脆弱性問題対策済みバージョンを使用しています。
- 3: 個人で使用するために作ったので、コードの作りはよくないかもです。コードの改編などは自由です。詳しくはLicenceというファイルを確認してください。
- 4: 下記の「機能3」と「機能4」はそれなりに強力にしたかったため、２重にAESをかけています。そのため、レスポンスはあまり良くない可能性があります。ただし、セーブ機能は高速化するために１回のみにしています。



# 機能
- 機能1: AES暗号化をしたデータをPlayerPrefsを利用して保存
- 機能2: 暗号化したデータを復元して読み込む
- 機能3: AES暗号化をした数値（int、string、float）をstringで返す
- 機能4: 「機能3」でAES暗号化したデータを復号化（型は暗号化した際の型で返ってきます）


# 使用方法
- １: このdllを使いたいプロジェクトに、dllをアタッチする。
- ２: 呼び出したいスクリプトに、下記の通りの文法で記述する。


### 保存(機能1)
例）//Key="保存名"、Security_key=”暗号化に必要なキーを生成するためのキー”、originalText=暗号化したいもの（int,string,float）
   UnitySaveAssist.Save("Key","Security_key",originalText);

### 読み込み(機能2)
例）//T=型の名前（intかfloatかstring）、Key="保存名"、Security_Key="暗号化に使用したキー"
   UnitySaveAssist.Load<T>("Key","Security_Key");

### 暗号化(機能3)
例）//Security_Key="暗号化に必要なキーを生成するためのキー"、variable=暗号化したいもの（int,float,string）
   UnitySaveAssist.Create_AES_text("Security_Key",variable);

### 復号化(機能4)
例）//AES_txt="暗号化した文字列"、AES_txt="「機能3」で暗号化したときの戻り値（string）"
   UnitySaveAssist.Decode_AES_text("Security_key",AES_txt);


## 製作者
[ふぐ](https://github.com/Hugu0141)
