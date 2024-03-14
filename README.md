# QuestionnaireForVRC
このプロジェクトはVRChatにおいてリッカート尺度を測定することを目的に作成されました。現在１１段階尺度まで対応しており、質問項目の数に上限はありません（VRChatやUdonSharpの制限については別途確認してください）。

This project was developed for the purpose of measuring Likert scales in VRChat. It currently supports up to an 11-point scale, there is no limit to the number of questions that can be asked (please check with VRChat and UdonSharp individually for their limitations).

## 使い方

1.UnityにVRChatSDK3-Worldを導入(Ver3.5で開発されました）

2.リリースから本プロジェクトのunitypakageを取得しUnityにインポート

3.QuestionnaireForVRC.prefabをSceneに配置

4.prefab内のCanvas>Script>Gimmickを選択しinspectorのPageGimmickから設定を行う

- StaffNameの設定
    - 結果の集計に用いるVRChatアカウントのIDをStaffNameを展開して出てくるリストに正確に入力（リストに追加されたアカウントのみ右側のパネルが表示され結果を確認できます）
- Questionnairesの設定
    - Questionnairesを展開し利用する質問紙のパターンの数をSizeに入力（５段階尺度,７段階尺,４段階尺度の３種類を用いるならば３）
    - 各Elementを展開し質問項目数+1をsizeに入力(20項目尺度であれば21)
    - Element0には回答方法などのリード文を入力（子の質問紙の全ページで同じリード文が表示されます）
    - Element1以降に質問項目を入力
- LabelTextsの設定
    - LabelTextsを展開し質問紙のパターンの数をSizeを入力（この数字はQuestionnairesのSizeと同一である必要があります）
    - 各Elementを展開し尺度の段階数を入力
    - Element0以降に尺度のラベルを入力（Element0:全くそう思わない,Element1:そう思わない…等）

# Usage
1. Introduce VRChatSDK3-World into Unity (developed in Ver3.5)
2. Get the unitpakage of this project from the release and import it into Unity

3.Place QuestionnaireForVRC.prefab in Scene

1. Select Canvas>Script>Gimmick in the prefab and configure settings from inspector's PageGimmick
- Set the StaffName
    - Enter the ID of the VRChat account that will be used to aggregate the results in the StaffName list (only the accounts added to the list will appear in the panel on the right to see the results)
- Setting up Questionnaires
    - Expand Questionnaires and enter the number of questionnaire patterns to be used in Size (3 if using 5-point scale, 7-point scale, or 4-point scale).
    - Expand each Element and enter the number of questionnaires + 1 in Size (21 for a 20-item scale).
    - Enter the lead sentence in Element 0 (the same lead sentence will appear on all pages of the child questionnaire).
    - Enter question items in Element1 and after.
- LabelTexts settings
    - Expand LabelTexts and enter the size of the number of patterns in the questionnaire (this number must be the same as the size of the Questionnaires).
    - Expand each Element and enter the number of steps in the scale.
    - Enter the scale labels after Element0 (Element0:Strongly disagree, Element1:Disagree...etc.)
