
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Serialization.OdinSerializer;
using Cysharp.Threading.Tasks.Triggers;

public class PageGimmick : UdonSharpBehaviour
{
    //ゲームオブジェクト読み込み
    //StartPanel関係
    public GameObject StartPanel;
    public GameObject StartButton;

    //MainPanel関係
    public GameObject MainPanel;
    public Text LeadText;
    public Text QuesText;
    public Toggle[] Toggles = new Toggle[11];
    public Text[] Labels = new Text[11];
    public GameObject NextPageButton;
    public GameObject PrevPageButton;


    //EndPanel関係
    public GameObject EndPanel;
    public GameObject EndButton;
    public GameObject EndPrevButton;

    //AnswerPanel関係
    public GameObject AnswerPanel;


    //事前設定変数定義
    [OdinSerialize] public string[][] Questionnaires; //質問紙定義配列要素0：リード文,要素1～n：質問紙内容
    [OdinSerialize] public string[][] LabelTexts;//質問紙のラベルを定義する変数
    public string[] StaffName = new string[] { "StaffID" };

    //システム変数定義
    public int[][] LocalAnswers;
    [UdonSynced] public string[] PublicAllAnswers;
    [UdonSynced] public int AnswerCount;
    public int QuesNum;
    public int PageNum;

    //PreFabの読み込み
    public GameObject AnswerListTMP;
    public GameObject AnswerContent;

    void Start()
    {
        AnswerReset();
        PageNum = 0;
        QuesNum = -1;
        PageCtrl();
        AnswerListCreate();
        PublicAllAnswersReset();
        AnswerPanelCtrl();
    }

    void Update()
    {
        AnswerListUpdate();
    }

    //Toggleのオンオフを切り替える
    public void ToggleActiveCtrl()
    {
        for (int i = 0; i < LabelTexts[QuesNum].Length; i++)
        {
            Toggles[i].gameObject.SetActive(true);
        }
        for (int i = LabelTexts[QuesNum].Length; i < Labels.Length; i++)
        {
            Toggles[i].gameObject.SetActive(false);
        }
    }

    //トグルのラベルを変更する
    public void LabelChange()
    {
        for (int i = 0; i < LabelTexts[QuesNum].Length; i++)
        {
            Labels[i].text = LabelTexts[QuesNum][i];
        }
    }

    //PageNumとQuesNumを制御する。入力iをPageNumに足し、PageNumの上限もしくは下限に達したQuesNumを変更
    public void NumCtrl(int i)
    {
        PageNum += i;
        if (PageNum <= 0)
        {
            QuesNum -= 1;
            if (QuesNum >= 0)
            {
                PageNum = Questionnaires[QuesNum].Length -1;
            }
            else
            {
                PageNum = 0;
            }

        }
        else if (PageNum >= Questionnaires[QuesNum].Length)
        {
            QuesNum += 1;
            PageNum = 1;

        }

    }

    //PageNumとQuesNumに応じて、表示内容を変更する
    public void PageCtrl()
    {
        if (QuesNum < 0)//質問紙の最初のページ
        {
            StartPanel.SetActive(true);
            MainPanel.SetActive(false);
            EndPanel.SetActive(false);
        }
        else if (QuesNum >= Questionnaires.Length)//質問紙の最後のページを超えた場合
        {
            StartPanel.SetActive(false);
            MainPanel.SetActive(false);
            EndPanel.SetActive(true);
        }
        else//質問紙回答途中のページ
        {
            StartPanel.SetActive(false);
            MainPanel.SetActive(true);
            EndPanel.SetActive(false);
            ToggleActiveCtrl();
            LabelChange();
            AnswerLoad();
            LeadText.text = Questionnaires[QuesNum][0];
            QuesText.text = Questionnaires[QuesNum][PageNum];
        }
    }

    //質問紙の回答を記録する
    public void AnswerRecord()
    {
        for (int i = 1; i < Toggles.Length+1; i++)
        {
            if (Toggles[i-1].isOn)
            {
                LocalAnswers[QuesNum][PageNum] = i;//回答を記録
                Toggles[i-1].isOn = false;//トグルをリセット
            }
        }
    }

    //保存された回答を呼び出しトグルに反映する
    public void AnswerLoad()
    {
        for (int i = 1; i < Toggles.Length+1; i++)
        {
            if (LocalAnswers[QuesNum][PageNum] == i)
            {
                Toggles[i-1].isOn = true;
            }
        }
    }

    //LocalAnswersをすべて0に初期化する
    public void AnswerReset()
    {
        LocalAnswers = new int[Questionnaires.Length][];
        for (int i = 0; i < Questionnaires.Length; i++)
        {
            LocalAnswers[i] = new int[Questionnaires[i].Length];
            for (int j = 0; j < Questionnaires[i].Length; j++)
            {
                LocalAnswers[i][j] = 0;
            }
        }
    }

    //スタートボタンを押したときの処理
    public void StartButtonPush()
    {
        Debug.Log("StartButtonPush");
        QuesNum = 0;
        PageNum = 1;
        PageCtrl();
    }
    //次のページボタンを押したときの処理
    public void NextPageButtonPush()
    {
        AnswerRecord();
        if (LocalAnswers[QuesNum][PageNum] != 0)
        {
            NumCtrl(1);
            PageCtrl();
        }
    }
    //前のページボタンを押したときの処理
    public void PrevPageButtonPush()
    {
        AnswerRecord();
        NumCtrl(-1);
        PageCtrl();
    }
    //最終ページの前に戻るボタンを押したときの処理
    public void EndPrevButtonPush()
    {
        AnswerRecord();
        NumCtrl(-1);
        PageCtrl();
    }

    //回答送信ボタンを押したときの処理
    public void EndButtonPush()
    {
        AnswerRecord();
        AnswerSave();
        AnswerCount += 1;
        PageNum = 0;
        QuesNum = -1;
        PageCtrl();
        AnswerReset();
    }


    //回答一覧のTMPオブジェクトをPrefabから生成する
    public void AnswerListCreate()
    {
        //localAnswersをもとに回答一覧のTMPオブジェクトをPrefabから生成する
        for (int i = 0; i < LocalAnswers.Length; i++)
        {
            for (int j = 1; j < LocalAnswers[i].Length; j++)
            {
                //AnswerListTMPを複製
                GameObject AnswerListTMPClone = Instantiate(AnswerListTMP, new Vector3(0, 0, 0), Quaternion.identity);
                //AnswerListTMPCloneの親をAnswersContentに設定
                AnswerListTMPClone.transform.SetParent(GameObject.Find("AnswersContent").transform, false);
                //AnswerListTMPCloneのTMPテキストを変更
                AnswerListTMPClone.GetComponent<TextMeshProUGUI>().text = "test";


            }
        }

    }

    //回答一覧更新メソッド
    public void AnswerListUpdate()
    {
        int PageCount = 0;
        //回答一覧を更新する
        for (int i = 0; i < LocalAnswers.Length; i++)
        {
            for (int j = 1; j < LocalAnswers[i].Length; j++)
            {
                //PageCount番目のAnsewerContentの子オブジェクトを取得
                GameObject AnswerContentChild = AnswerContent.transform.GetChild(PageCount).gameObject;
                AnswerContentChild.GetComponent<TextMeshProUGUI>().text = PublicAllAnswers[PageCount];
                PageCount += 1;
            }
        }
    }


    //publicAllAnswersを初期化する
    public void PublicAllAnswersReset()
    {
        int QuesAmount = 0;
        for (int i = 0; i < Questionnaires.Length; i++)
        {
            QuesAmount += Questionnaires[i].Length - 1;
        }
        PublicAllAnswers = new string[QuesAmount];
        AnswerCount = 0;
        for (int i = 0; i < LocalAnswers.Length; i++)
        {
            for (int j = 1; j < LocalAnswers[i].Length; j++)
            {
                if (PublicAllAnswers[AnswerCount] == null)
                {
                    PublicAllAnswers[AnswerCount] = "Q" + (i + 1) + "-" + j + "\r\n";
                    AnswerCount += 1;
                }

            }
        }
    }


    //LocalAnswersの内容をPublicAllAnswersに保存する
    public void AnswerSave()
    {
        if (!Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        int AnsCount = 0;
        for (int i = 0; i < LocalAnswers.Length; i++)
        {
            for (int j = 1; j < LocalAnswers[i].Length; j++)
            {
                PublicAllAnswers[AnsCount] += LocalAnswers[i][j].ToString() + "\r\n";
                AnsCount += 1;
            }
        }
        RequestSerialization();

    }



    //AnswerPanelのオンオフ制御
    public void AnswerPanelCtrl()
    {
        for (int i = 0; i < StaffName.Length; i++)
        {
            if (Networking.LocalPlayer.displayName == StaffName[i])
            {
                AnswerPanel.SetActive(true);
            }
            else
            {
                AnswerPanel.SetActive(false);
            }
        }
    }

    //プレイヤーが新規に参加したときの処理
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }



}
