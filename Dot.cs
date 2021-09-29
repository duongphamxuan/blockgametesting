using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variable")]
    public int column, row, previousColumn, previousRow;
    Vector2 firstTouchPosition, finalTouchPosition, tempPosition;
    float swipeAngle = 0;
    float swipeResist = 0.5f;
    public bool isMatched = false;

    [Header("GameObject")]
    Board board;
    GameObject otherDot;



    void Start()
    {
        board = FindObjectOfType<Board>();
        column = (int) transform.position.x;
        row = (int) transform.position.y;
        previousColumn = column;
        previousRow = row;
    }

    void Update()
    {
        RightLeftMoving();
        UpDownMoving();

        FindMatches();
        if(isMatched)
        {
            AutomaticDown();
        }
    }

    private void RightLeftMoving()
    {
        if (Mathf.Abs(column - transform.position.x) > 0.1)
        {
            //Move toward the target
            tempPosition = new Vector2(column, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
            if(board.allDots[column, row] != this.gameObject) 
            {
                board.allDots[column,row] = this.gameObject; // update new position
            }
                
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(column, transform.position.y);
            transform.position = tempPosition;
        }
    }

    private void UpDownMoving()
    {
        if (Mathf.Abs(row - transform.position.y) > 0.1)
        {
            //Move toward the target
            tempPosition = new Vector2(transform.position.x, row);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject; // update new position
            }
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, row);
            transform.position = tempPosition;
        }
    }

    private void OnMouseDown() {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp() {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle(){
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist ||
           Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist ) 
           {
                //Calculator the mouse position in right, left, up or down of object pointed to angle
                swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
                // Debug.Log(swipeAngle);
                MovePieces();
           }
    }

    void MovePieces(){
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width -1) //Right swipe and not out of board
        {
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column-=1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1) //Up swipe and not out of board
        {
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if (swipeAngle > 135 || swipeAngle <= -135 && column > 0) //left swipe and not out of board
        {
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle > -135 && row > 0) //Down swipe and not out of board
        {
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(0.5f);
        if (otherDot != null)
        { // To check gameObject can move to otherDot's position
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            { //but if gameObject not match with left, right, down upDot, then
                // return back position of otherDot
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                // return back position of gameObject
                row = previousRow;
                column = previousColumn;
            }
            otherDot = null;
        }
    }

    void FindMatches(){
        if (column > 0 && column < board.width -1 )
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if(leftDot1 != null && rightDot1 != null)
            {
                if(leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag) 
                {
                    Destroy(leftDot1);
                    Destroy(rightDot1);
                    Destroy(this.gameObject);
                    isMatched = true;
                }
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject DownDot1 = board.allDots[column, row - 1];
            GameObject UpDot1 = board.allDots[column, row + 1];
            if (DownDot1 != null && UpDot1 != null)
            {
                if (DownDot1.tag == this.gameObject.tag && UpDot1.tag == this.gameObject.tag)
                {
                    Destroy(DownDot1);
                    Destroy(UpDot1);
                    Destroy(this.gameObject);
                    isMatched = true;
                }
            }

        }
    }

    private void AutomaticDown()
    {
        GameObject DownSlot = board.allDots[column, row -1];
        if(DownSlot == null)
        {
            row -= 1;
        }
    }
}
