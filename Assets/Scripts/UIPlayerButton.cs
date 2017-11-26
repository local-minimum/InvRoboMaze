using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerButton : MonoBehaviour {

    [SerializeField]
    Board board;

    [SerializeField]
    int playerId = 0;
    Button btn;
    Text txt;

    public int PlayerId
    {
        get
        {
            return playerId;
        }
    }

    private void Start()
    {

        btn = GetComponent<Button>();
        txt = GetComponentInChildren<Text>();
        SetDoRoll();
    }

    public void SetRollResults(int board, int moves)
    {
        txt.text = string.Format(
            "Board: {0} Player: {1}",
            board,
            moves
        );
    }

    public void SetDoRoll()
    {
        txt.text = string.Format(
            "Roll Player {0}",
            playerId + 1
        );
    }
}
