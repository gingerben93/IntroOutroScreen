using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Main : MonoBehaviour
{
    public const float size = .5f;

    //variables
    public int height = 0;

    //side maxes
    public float leftMax = -3.75f;
    public float rightMax = 3.75f;
    public float forwardMax = 3.75f;
    public float backMax = -3.75f;

    //gameObjects
    public Transform rightWall;
    public Transform leftWall;
    public Transform cylinderColor1;
    public Transform cylinderColor2;
    public Transform cylinderColor3;
    public Transform ElbowColor1;
    public Transform ElbowColor2;
    public Transform ElbowColor3;

    //current set of tubes
    public Transform StraightTubes;
    public Transform CurrentTubeSet;
    public Transform Tubes1;
    public Transform Tubes2;

    public HashSet<Vector3> TubeLocations  = new HashSet<Vector3>();

    //Gameobject containers for holding precedural gen objects
    public Transform TubeSetContainer;

    //temp counter for saving objects
    int counter = 0;

    enum MoveOptions
    {
        left,
        right,
        forward,
        back,
        up,
        down
    }


    // Use this for initialization
    void Start()
    {
        TubeSetContainer = GameObject.Find("TubeSetContainer").transform;
        StraightTubes = GameObject.Find("StraightTubes").transform;
        Tubes1 = GameObject.Find("Tubes1").transform;
    }

    void MakeTubes(Transform CurrentCylinder, Transform CurrentElbow)
    {
        //straight move first
        int startRange = 6;
        int endRange = 13;

        MoveOptions prevDir = MoveOptions.up;

        //keep track of current position
        Vector3 curVec = Vector3.zero;

        curVec = EnsureDifferentStartLocation(curVec);

        TubeLocations.Add(curVec);

        int tempCounter = 0;

        //until tubes are certain height
        while (curVec.y < height)
        {
            int dis = Random.Range(1, 3);
            int dir = Random.Range(startRange, endRange);
            MoveOptions direction = MoveOptions.up;

            if (dir >= 0 && dir <= 2)
            {
                direction = MoveOptions.left;

                //cant move futher then max in that direction
                if (curVec.x <= leftMax)
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

            Vector3 checkIfOtherPipe = Vector3.zero;
            checkIfOtherPipe = CanMakeElbowThenForward(curVec, direction, prevDir);
            if (!TubeLocations.Contains(checkIfOtherPipe))
            {
                //dont add elbow if last move was straight
                if (prevDir != direction)
                {
                    curVec = MakeElbow(direction, prevDir, curVec, CurrentElbow);
                    //change current position to end of last move 
                    curVec = MakeStraightTubes(dis, direction, prevDir, curVec, CurrentCylinder);
                }
                else
                {
                    //if no elbow move, add one to distance to make plus one stright move
                    dis += 1;
                    //change current position to end of last move 
                    curVec = MakeStraightTubes(dis, direction, prevDir, curVec, CurrentCylinder);
                }

                //set prev to current at end
                prevDir = direction;
            }
            else
            {
                //print("was in");
                //print(checkIfOtherPipe);
                //print(curVec);
            }
            tempCounter++;
            if (tempCounter >= 1000)
                break;
        }
    }

    Vector3 EnsureDifferentStartLocation(Vector3 tubeStartLocation)
    {
        float rightShift = 0;
        float leftShift = 0;
        float backShift = 0;
        float forwardShift = 0;

        //ensures different start location for each tube
        while (TubeLocations.Contains(tubeStartLocation))
        {
            int randMoveLocation = Random.Range(0, 4);
            if (randMoveLocation == 0)
            {
                rightShift += .5f;
                tubeStartLocation = new Vector3(rightShift, 0, 0);
            }
            else if (randMoveLocation == 1)
            {
                leftShift -= .5f;
                tubeStartLocation = new Vector3(leftShift, 0, 0);
            }
            else if (randMoveLocation == 2)
            {
                backShift -= .5f;
                tubeStartLocation = new Vector3(0, 0, backShift);
            }
            else if (randMoveLocation == 3)
            {
                forwardShift += .5f;
                tubeStartLocation = new Vector3(0, 0, forwardShift);
            }
        }

        return tubeStartLocation;
    }

    bool CheckIfCanMoveForward(Vector3 curVec, MoveOptions direction)
    {
        Vector3 CollideVec = curVec;

        if (direction == MoveOptions.left)
        {
            CollideVec = new Vector3(CollideVec.x - size * 2, CollideVec.y, CollideVec.z);
        }
        else if (direction == MoveOptions.right)
        {
            CollideVec = new Vector3(CollideVec.x + size * 2, CollideVec.y, CollideVec.z);
        }
        else if (direction == MoveOptions.up)
        {
            CollideVec = new Vector3(CollideVec.x, CollideVec.y + size * 2, CollideVec.z);
        }
        else if (direction == MoveOptions.forward)
        {
            CollideVec = new Vector3(CollideVec.x, CollideVec.y, CollideVec.z + size * 2);
        }
        else if (direction == MoveOptions.back)
        {
            CollideVec = new Vector3(CollideVec.x, CollideVec.y, CollideVec.z - size * 2);
        }

        if (TubeLocations.Contains(CollideVec))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    Vector3 CanMakeElbowThenForward(Vector3 curVec, MoveOptions direction, MoveOptions prevDirection)
    {
        Vector3 CollideVec = curVec;

        if (direction == MoveOptions.left)
        {
            if (prevDirection == MoveOptions.up)
            {
                CollideVec = new Vector3(CollideVec.x - size, CollideVec.y + size, CollideVec.z);
            }
            else if (prevDirection == MoveOptions.forward)
            {
                CollideVec = new Vector3(CollideVec.x - size, CollideVec.y, CollideVec.z + size);
            }
            else if (prevDirection == MoveOptions.back)
            {
                CollideVec = new Vector3(CollideVec.x - size, CollideVec.y, CollideVec.z - size);
            }
        }
        else if (direction == MoveOptions.right)
        {
            if (prevDirection == MoveOptions.up)
            {
                CollideVec = new Vector3(CollideVec.x + size, CollideVec.y + size, CollideVec.z);
            }
            else if (prevDirection == MoveOptions.forward)
            {
                CollideVec = new Vector3(CollideVec.x + size, CollideVec.y, CollideVec.z + size);
            }
            else if (prevDirection == MoveOptions.back)
            {
                CollideVec = new Vector3(CollideVec.x + size, CollideVec.y, CollideVec.z - size);
            }
        }
        else if (direction == MoveOptions.up)
        {
            if (prevDirection == MoveOptions.left)
            {
                CollideVec = new Vector3(CollideVec.x - size, CollideVec.y + size, CollideVec.z);
            }
            else if (prevDirection == MoveOptions.right)
            {
                CollideVec = new Vector3(CollideVec.x + size, CollideVec.y + size, CollideVec.z);
            }
            else if (prevDirection == MoveOptions.forward)
            {
                CollideVec = new Vector3(CollideVec.x, CollideVec.y + size, CollideVec.z + size);
            }
            else if (prevDirection == MoveOptions.back)
            {
                CollideVec = new Vector3(CollideVec.x, CollideVec.y + size, CollideVec.z - size);
            }
        }
        else if (direction == MoveOptions.forward)
        {
            if (prevDirection == MoveOptions.left)
            {
                CollideVec = new Vector3(CollideVec.x - size, CollideVec.y, CollideVec.z + size);
            }
            else if (prevDirection == MoveOptions.right)
            {
                CollideVec = new Vector3(CollideVec.x + size, CollideVec.y, CollideVec.z + size);
            }
            else if (prevDirection == MoveOptions.up)
            {
                CollideVec = new Vector3(CollideVec.x, CollideVec.y + size, CollideVec.z + size);
            }
        }
        else if (direction == MoveOptions.back)
        {
            if (prevDirection == MoveOptions.left)
            {
                CollideVec = new Vector3(CollideVec.x - size, CollideVec.y, CollideVec.z - size);
            }
            else if (prevDirection == MoveOptions.right)
            {
                CollideVec = new Vector3(CollideVec.x + size, CollideVec.y, CollideVec.z - size);
            }
            else if (prevDirection == MoveOptions.up)
            {
                CollideVec = new Vector3(CollideVec.x, CollideVec.y + size, CollideVec.z - size);
            }
        }

        return CollideVec;
    }


    //places elbows correctly
    Vector3 MakeElbow(MoveOptions diretion, MoveOptions prevDirection, Vector3 curVec, Transform CurrentElbow)
    {
        var temp = Instantiate(CurrentElbow, CurrentTubeSet);
        temp.name = "Elbow" + counter;
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

    Vector3 MakeStraightTubes(int distance, MoveOptions direction, MoveOptions prevDirection, Vector3 curVec, Transform CurrentCylinder)
    {
        //start at one becuase elbow counts as first move
        for (int x = 1; x < distance; x++)
        {

            //check if can make straight move
            if (!CheckIfCanMoveForward(curVec, direction))
            {
                print(curVec);
                break;
            }

            var temp = Instantiate(CurrentCylinder, CurrentTubeSet);
            temp.name = "Straight" + counter;
            if (direction == MoveOptions.left)
            {
                curVec = new Vector3(curVec.x - size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (direction == MoveOptions.right)
            {
                curVec = new Vector3(curVec.x + size, curVec.y, curVec.z);
                temp.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (direction == MoveOptions.up)
            {
                curVec = new Vector3(curVec.x, curVec.y + size, curVec.z);
                temp.eulerAngles = new Vector3(0, 0, 90);
            }
            else if (direction == MoveOptions.forward)
            {
                curVec = new Vector3(curVec.x, curVec.y, curVec.z + size);
                temp.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (direction == MoveOptions.back)
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
            foreach(Transform sets in TubeSetContainer)
            {
                foreach (Transform child in sets)
                {
                    TubeLocations.Clear();
                    Destroy(child.gameObject);
                }
            }

            //calculate cylinder size scale to unit length
            float sizeCylinder = cylinderColor2.transform.localScale.x * 2;

            //have min highet
            if (height <= 0)
            {
                height = 1;
            }

            //stright up pipe
            for (int y = 0; y < (int)(height / sizeCylinder); y++)
            {
                var tempTube = Instantiate(cylinderColor3, StraightTubes);
                tempTube.name = "straight" + y;
                tempTube.transform.position = new Vector3(0, y * sizeCylinder, 0);
                TubeLocations.Add(tempTube.transform.position);
            }

            //make tube system
            GameObject tubeset1 = new GameObject();
            tubeset1.name = "tubeset1";
            tubeset1.transform.SetParent(TubeSetContainer);
            CurrentTubeSet = tubeset1.transform;
            MakeTubes(cylinderColor1, ElbowColor1);

            GameObject tubeset2 = new GameObject();
            tubeset2.name = "tubeset2";
            tubeset2.transform.SetParent(TubeSetContainer);
            CurrentTubeSet = tubeset2.transform;
            MakeTubes(cylinderColor2, ElbowColor2);

            //MakeTubes(cylinderColor3, ElbowColor3);
        }

        
        //just drag and drop the parent object to save this works way better. saves all the lights an everything

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    MeshFilter[] meshFilters = Tubes1.GetComponentsInChildren<MeshFilter>();
        //    CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        //    print(meshFilters.Length);
        //    int i = 0;
        //    while (i < meshFilters.Length)
        //    {
        //        print(i);
        //        combine[i].mesh = meshFilters[i].sharedMesh;
        //        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //        meshFilters[i].gameObject.SetActive(false);
        //        i++;
        //    }

        //    //Seting up the gameobject
        //    GameObject test = new GameObject();
        //    test.name = "CombinedTube" + counter;
        //    test.AddComponent<MeshFilter>();
        //    test.AddComponent<MeshRenderer>();

        //    test.GetComponent<MeshFilter>().mesh = new Mesh();
        //    test.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        //    test.gameObject.SetActive(true);

        //    string SavePath = "Assets/Shapes/ConstructScene/TempShapes/CombinedTubeMeshFilter" + counter + ".assest";

        //    AssetDatabase.CreateAsset(test.GetComponent<MeshFilter>().mesh, SavePath);
        //    AssetDatabase.SaveAssets();

        //    counter++;
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    MeshFilter[] meshFilters = StraightTubes.GetComponentsInChildren<MeshFilter>();
        //    CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        //    int i = 0;

        //    //65k vertices limit for single mesh
        //    while (i < meshFilters.Length)
        //    {
        //        combine[i].mesh = meshFilters[i].sharedMesh;
        //        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //        meshFilters[i].gameObject.SetActive(false);
        //        i++;
        //    }

        //    //Seting up the gameobject
        //    GameObject test = new GameObject();
        //    test.name = "StraightTubes";
        //    test.AddComponent<MeshFilter>();
        //    test.AddComponent<MeshRenderer>();

        //    test.GetComponent<MeshFilter>().mesh = new Mesh();
        //    test.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        //    test.gameObject.SetActive(true);

        //    string SavePath = "Assets/Shapes/ConstructScene/TempShapes/StraightTubeMeshFilter.assest";

        //    AssetDatabase.CreateAsset(test.GetComponent<MeshFilter>().mesh, SavePath);
        //    AssetDatabase.SaveAssets();
        //}
    }
}
