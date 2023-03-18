using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



///////////////////////////////////////////////////////////////////////////////////////
//────────────────────────────
//│ 0│ 1│ 0│ 0│ 2│ 0│ 0│ 2│ 0│ 
//────────────────────────────
//────────────────────────────
//│ 1│ 0│ 0│ 2│ 0│ 0│ 2│ 0│ 0│ 
//────────────────────────────
//────────────────────────────
//│ 0│ 0│ 1│ 0│ 0│ 2│ 0│ 0│ 2│ 
//────────────────────────────
//위 세가지의 레시피가 모두 같은 결과가 나와야 한다. 
//레시피를 9개 크기의 레시피로 저장하는것이 아닌 레시피의 시작부터 끝까지의 패턴으로만 저장한다.
//──────────────────────
//│ 1│ 0│ 0│ 2│ 0│ 0│ 2│ //필요없는 부분을 지우고 필요한 부분만 저장
//──────────────────────
///////////////////////////////////////////////////////////////////////////////////////

//물건 하나에 대한 레시피
[System.Serializable]
public class CraftRecipe
{
    
    public int[] Pattern;
    public int result;
    public int stock;
    public int patternsize;


    //레시피 패턴을 만들어 준다.
    public CraftRecipe(int result, int resultstock,int[] recipe)
    {
        this.result = result;
        this.stock = resultstock;
        this.patternsize = 0;
        int index = 0;
        int startindex = -1;
        int endindex = -1;
        for(int i=0; i<recipe.Length;i++)
        {
            if (recipe[i] != 0 && endindex <= i)
            {
                if (startindex == -1)
                    startindex = i;
                endindex = i; 
            }
        }
        patternsize = endindex - startindex + 1;


        Pattern = new int[patternsize];
        for (int i = startindex; i <= endindex; i++)
        {
            Pattern[index++] = recipe[i];
        }

        //studentList.Sort((x1, x2) => x2.Weight.CompareTo(x1.Weight));
    }

    public bool IsEqual(int[] pattern)
    {
        if (this.patternsize != pattern.Length)
        {
            Debug.Log("잘못된 비교");
            return false;
        }
        for(int i=0;i<patternsize;i++)
        {
            if(this.Pattern[i]!=pattern[i])
            {
                return false;
            }
        }
        return true;
    }


    static public int[] GetPattern(int[] input)
    {
        int[] temp;

        int size = 0;
        int index = 0;
        int startindex = -1;
        int endindex = -1;

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] != 0 && endindex <= i)
            {
                if (startindex == -1)
                    startindex = i;
                endindex = i;
            }
        }
        size = endindex - startindex + 1;

        temp = new int[size];
        for (int i = startindex; i <= endindex; i++)
        {
            temp[index++] = input[i];
        }

        return temp;
    }

    //입력받은 레시피를 똑같이 패턴으로 바꾼다음에 1차적으로 패턴의 크기를 비교하고 2차적으로 내용을 비교하여 해당 레시피를 탐색한다. 1번째 탐색을 할때는 이진탐색을 이용한다.
    static public BaseNode SearchRecipe(List<CraftRecipe> recipelist, int[] input)
    {
        BaseNode node = null;
        int[] inputpattern = GetPattern(input);

        //같은 크기의 레시피들을 뽑아서 비교한다.
        List<CraftRecipe> temparr =  recipelist.FindAll(x => x.patternsize == inputpattern.Length);

        for(int i=0;i<temparr.Count;i++)
        {
            if (temparr[i].IsEqual(inputpattern))
            {
                Debug.Log($"찾음 {temparr[i].result}");

                node = GameObject.Instantiate<BaseNode>(ItemDataLoader.Instance.itemnodes.Find(x => x.GetItemID() == temparr[i].result));
                node.ChangeStack(0);
                node.ChangeStack(temparr[i].stock);
                node.NodeIsActive = true;
                return node;
                //return temparr[i].result;
            }
        }

        Debug.Log($"못찾음");
        return node;
    }


}


//레시피 파일을 읽어온다.
public class CraftDataLoader : MonoBehaviour
{
    public List<string> originalstr = null;
    public string FilePath;

    public List<BaseNode> itemnodes = new List<BaseNode>();
    public Sprite[] sprites = null;

    public List<CraftRecipe> recipelist = new List<CraftRecipe>();


    public List<Dictionary<int, string>> CraftFileOpen(string filepath, out string classname)
    {
        originalstr.Clear();
        List<Dictionary<int, string>> data = new List<Dictionary<int, string>>();
        string FilePath = filepath;
        Debug.Log($"road {FilePath}");
        classname = null;

        //if (OpenDialog.ShowDialog() == DialogResult.OK)
        //{
        //    if ((openStream = OpenDialog.OpenFile()) != null)
        //    {
        //        //return OpenDialog.FileName;
        //        FilePath = OpenDialog.FileName;
        //    }
        //}
        if (FilePath == null)
        {
            Debug.Log("여기서 나감1");
            return null;
        }


        FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);

        StreamReader sr = new StreamReader(fs);

        while (true)
        {
            string str = sr.ReadLine();
            originalstr.Add(str);

            if (str == null || str.Length == 0)
            {
                break;
            }
            char[] temp = new char[str.Length];
            bool flag = false;

            Dictionary<int, string> columsdic = new Dictionary<int, string>();
            int Dicindex = 0;
            int index = 0;
            //string temp;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '{')
                {
                    flag = true;//여는중괄호가 나오면 닫는중괄호가 나올때까지 앞으로 나오는 , 는 그냥 집어넣는다.  
                    continue;
                }
                else if (str[i] == ',')//,가 나오면 해당 인덱스에는 , string으로 변환해서 columsdic 에 넣어준다.
                {
                    if (!flag)
                    {
                        temp[index] = '\0';
                        string tt = string.Join("", temp);//
                        columsdic.Add(Dicindex++, tt.Split('\0')[0]);//그냥 new string() 생성자를 이용하면 char배열의 크기만큼 뒤에 \0으로 초기화된 문자열이 만들어져버린다.

                        temp = new char[str.Length];
                        index = 0;
                        continue;
                    }
                }
                else if (i == str.Length - 1)//받아온 한 줄의 마지막 문자일때도 쉼표과 같은 동작을 해준다.
                {
                    if (str[i] != '}')//만약 마지막이 } 로 끝났을때는 }문자도 넣어준다.
                        temp[index++] = str[i];

                    temp[index] = '\0';
                    string tt = string.Join("", temp);
                    columsdic.Add(Dicindex++, tt.Split('\0')[0]);//그냥 new string() 생성자를 이용하면 char배열의 크기만큼 뒤에 \0으로 초기화된 문자열이 만들어져버린다.

                    temp = new char[str.Length];
                    index = 0;
                    break;
                }
                else if (str[i] == '}')
                {
                    flag = false;
                    continue;
                }


                temp[index++] = str[i];

            }


            int RowNum = 0;

            if (int.TryParse(columsdic[0], out RowNum))//첫번째가 숫자가 아니면 해당 행은 열제목 행이다. 리스트에 넣지 않는다.
            {
                data.Add(columsdic);
            }
            else
            {
                //classname = columsdic[0].Split('_')[0];
            }

        }
        return data;
    }

    public void CraftInfo_Road(string classname)
    {
        Debug.Log(FilePath);
        string temppath = Application.streamingAssetsPath + FilePath;//런타임중 읽기만 가능
        List<Dictionary<int, string>> datalist = CraftFileOpen(temppath, out classname);
        int[] recipe = new int[9];
        int result;
        int stock;
        
        //List<CraftRecipe> recipelist = new List<CraftRecipe>();
        //들어온 정보들을 가지고 레시피 리스트를 만들어 준다.
        for (int i = 0; i < datalist.Count; i++)
        {
            int.TryParse(datalist[i][(int)EnumTypes.RecipeCollums.Count], out stock);
            int.TryParse(datalist[i][(int)EnumTypes.RecipeCollums.ResultItem], out result);
            for (EnumTypes.RecipeCollums j = EnumTypes.RecipeCollums.Slot1;j<=EnumTypes.RecipeCollums.Slot9;j++)
            {
                int.TryParse(datalist[i][(int)j], out recipe[(int)j - 3]);
            }
            recipelist.Add(new CraftRecipe(result, stock, recipe));
        }
        recipelist.Sort((x1, x2) => x1.patternsize.CompareTo(x2.patternsize));//크기순으로 오름차순 정렬

        ItemCraftTable.Instance.recipelist = recipelist;

    }



    void Start()
    {
        CraftInfo_Road(FilePath);
    }


}
