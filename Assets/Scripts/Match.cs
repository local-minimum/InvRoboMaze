using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PlayerOrder : IEnumerable<int>
{
    int[] order;
    int nPlayers;

    public PlayerOrder(int nPlayers)
    {
        order = new int[nPlayers];
        for(int i=0; i<nPlayers; i++)
        {
            order[i] = i;
        }
        this.nPlayers = nPlayers;
    }

    public void Shuffle()
    {
        int n = nPlayers - 1;
        while (n > 1)
        {
            int k = UnityEngine.Random.Range(0, nPlayers);
            int val = order[k];
            order[k] = order[n];
            order[n] = val;
        }
        
    }

    public void RotateLeft()
    {
        int left = order[0];
        for (int i = 1; i < nPlayers; i++)
        {
            order[i - 1] = order[i];
        }
        order[nPlayers - 1] = left;
    }

    public IEnumerator<int> GetEnumerator()
    {
        for (int i = 0; i < nPlayers; i++)
        {
            yield return order[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < nPlayers; i++)
        {
            yield return order[i];
        }
    }

}

public class Match : MonoBehaviour {

    public int actionPointsPerPlayer = 7;

    public int nPlayers = 2;

    public int[] actionPoints;
    PlayerOrder playerOrder;
    Player[] players;

    private void Start()
    {
        playerOrder = new PlayerOrder(nPlayers);
        playerOrder.Shuffle();

        actionPoints = new int[nPlayers];
        RefillActionPoints();
    }

    public void SetPlayers(params Player[] players)
    {
        this.players = players;
    }

    public void RefillActionPoints()
    {
        for (int i=0; i<actionPoints.Length; i++)
        {
            actionPoints[i] = actionPointsPerPlayer;
        }
        
    }

    bool playerMoves = false;

    public void MovePlayers()
    {
        if (!playerMoves)
        {
            StartCoroutine(_MovePlayers());
        }
    }

    IEnumerator<WaitForSeconds> _MovePlayers() {
        playerMoves = true;
        while (true)
        {
            bool anyMove = false;
            foreach (int player in playerOrder)
            {
                if (actionPoints[player] < 1)
                {
                    continue;
                }

                players[player].Move();
                anyMove = true;
                actionPoints[player]--;
                yield return new WaitForSeconds(0.5f);
            }
            if (!anyMove)
            {
                break;
            }
        }
        playerMoves = false;
    }
}
