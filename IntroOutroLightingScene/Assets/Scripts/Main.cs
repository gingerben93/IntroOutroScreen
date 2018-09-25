using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    //variables
    public int height = 0;

    //gameObjects
    public Transform rightWall;
    public Transform leftWall;
    public Transform cylinderColor1;
    public Transform cylinderColor2;
    public Transform ElbowColor1;
    public Transform EblowColor2;

    enum MoveOptions
    {
        left,
        right,
        up,
        down,
        forward,
        back
    }


    // Use this for initialization
    void Start()
    {
        //calculate cylinder size scale to unit length
        float size = cylinderColor2.transform.localScale.x * 2;
        if (height <= 0)
        {
            height = 1;
        }

        for(int y = 0; y < (int)(height/size); y++)
        {
            var temp = Instantiate(cylinderColor2);
            temp.transform.position = new Vector3(0, y * size, 0);
        }

        int startRange = 4;
        int endRange = 15;

        //side maxes
        float leftMax = -.75f;
        float rightMax = .75f;
        float forwardMax = .75f;
        float backMax = -.75f;

        MoveOptions prevDir = MoveOptions.up;

        Vector3 curVec = Vector3.zero;
        while(curVec.y < height)
        {
            int dis = Random.Range(1, 5);
            int dir = Random.Range(startRange, endRange);
            MoveOptions direction = MoveOptions.up;

            if (dir >= 0 && dir <= 2)
            {
                direction = MoveOptions.left;

                //cant move futher then max in that direction
                if(curVec.x <= leftMax)
                {
                    direction = MoveOptions.right;
                }

                //stop two x axis moves in a row
                startRange = 6;
            }
            else if (dir >= 3 && dir <= 5)
            {
                direction = MoveOptions.right;

                //cant move futher then max in that direction
                if (curVec.x >= rightMax)
                {
                    direction = MoveOptions.left;
                }

                //stop two x axis moves in a row
                startRange = 6;
            }
            else if (dir >= 6 && dir <= 13)
            {
                direction = MoveOptions.up;

                //allow z and x axis moves again
                startRange = 0;
                endRange = 20;
            }
            //else if (false)
            //{
            //    moveOps = (int)MoveOptions.down;
            //      startRange = 0;
            //      endRange = 20;
            //}
            else if (dir >= 14 && dir <= 16)
            {
                direction = MoveOptions.forward;

                //cant move futher then max in that direction
                if (curVec.z >= forwardMax)
                {
                    direction = MoveOptions.back;
                }

                //stop two z axis moves in a row
                endRange = 13;
            }
            else if (dir >= 17 && dir <= 19)
            {
                direction = MoveOptions.back;

                //cant move futher then max in that direction
                if (curVec.z <= backMax)
                {
                    direction = MoveOptions.forward;
                }

                //stop two z axis moves in a row
                endRange = 13;
            }

            curVec = Move(dis, direction, prevDir, curVec);
            prevDir = direction;
        }
    }

    //places elbows correctly
    Vector3 MakeElbow(MoveOptions diretion, MoveOptions prevDirection, Vector3 curVec)
    {
        float size = .5f;
        var temp = Instantiate(ElbowColor1);

        if (diretion == MoveOptions.left)
        {
            temp.eulerAngles = new Vector3(0, 0, 0);
            if (prevDirection == MoveOptions.up)
            {
                curVec = new Vector3(curVec.x, curVec.y + size, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 180, temp.eulerAngles.y, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.forward)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z + size);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 270, temp.eulerAngles.y, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.back)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z - size);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 90, temp.eulerAngles.y, temp.eulerAngles.z);
            }
        }
        else if (diretion == MoveOptions.right)
        {
            temp.eulerAngles = new Vector3(0, 180, 0);
            if (prevDirection == MoveOptions.up)
            {
                curVec = new Vector3(curVec.x, curVec.y + size, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 180, temp.eulerAngles.y, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.forward)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z + size);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 90, temp.eulerAngles.y, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.back)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z - size);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 270, temp.eulerAngles.y, temp.eulerAngles.z);
            }
        }
        else if (diretion == MoveOptions.up)
        {
            temp.eulerAngles = new Vector3(0, 0, 0);
            if (prevDirection == MoveOptions.left)
            {
                curVec = new Vector3(curVec.x - size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y + 180, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.right)
            {
                curVec = new Vector3(curVec.x + size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (prevDirection == MoveOptions.forward)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z + size);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y + 270, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.back)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z - size);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x, temp.eulerAngles.y + 90, temp.eulerAngles.z);
            }
        }
        else if (diretion == MoveOptions.forward)
        {
            temp.eulerAngles = new Vector3(0, 90, 0);
            if (prevDirection == MoveOptions.left)
            {
                curVec = new Vector3(curVec.x - size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 90, temp.eulerAngles.y, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.right)
            {
                curVec = new Vector3(curVec.x + size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 270, temp.eulerAngles.y, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.up)
            {
                curVec = new Vector3(curVec.x, curVec.y + size, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 180, temp.eulerAngles.y, temp.eulerAngles.z);
            }
        }
        else if (diretion == MoveOptions.back)
        {
            temp.eulerAngles = new Vector3(0, 270, 0);
            if (prevDirection == MoveOptions.left)
            {
                curVec = new Vector3(curVec.x - size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 270, temp.eulerAngles.y, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.right)
            {
                curVec = new Vector3(curVec.x + size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 90, temp.eulerAngles.y, temp.eulerAngles.z);
            }
            else if (prevDirection == MoveOptions.up)
            {
                curVec = new Vector3(curVec.x, curVec.y + size, curVec.z);
                temp.eulerAngles = new Vector3(temp.eulerAngles.x + 180, temp.eulerAngles.y, temp.eulerAngles.z);
            }
        }

        temp.transform.position = curVec;
        return curVec;
    }

    Vector3 Move(int distance, MoveOptions diretion, MoveOptions prevDirection, Vector3 curVec)
    {

        float size = .5f;
        print("elbow");
        print(distance);

        //dont add elbow if last move was straight
        if (prevDirection != diretion)
        {
            curVec = MakeElbow(diretion, prevDirection, curVec);
        }
        else
        {
            //if no elbow move, add one to distance to make plus one stright move
            distance += 1;
        }

        //start at one becuase elbow counts as first move
        for (int x = 1; x < distance; x++)
        {
            var temp = Instantiate(cylinderColor1);
            if (diretion == MoveOptions.left)
            {
                curVec = new Vector3(curVec.x - size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (diretion == MoveOptions.right)
            {
                curVec = new Vector3(curVec.x + size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (diretion == MoveOptions.up)
            {
                curVec = new Vector3(curVec.x, curVec.y + size, curVec.z);
                temp.eulerAngles = new Vector3(0, 0, 90);
            }
            else if (diretion == MoveOptions.forward)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z + size);
                temp.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (diretion == MoveOptions.back)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z - size);
                temp.eulerAngles = new Vector3(0, 90, 0);
            }
            temp.transform.position = curVec;
        }
        return curVec;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

        }
    }
}
