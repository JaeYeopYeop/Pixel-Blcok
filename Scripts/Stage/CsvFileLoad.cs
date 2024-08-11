using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageData
{
    public int StageNum { set; get; }   // 스테이지 번호
    public string CategoryName { set; get; } //폴더명
    public string ImageName { set; get; }   // 이미지 파일이름
    public int DropColorCount { set; get; } // 드롭 블럭 갯수(전체 스테이지 컬러중에 몇개의 color의 color 블럭을 남길건지)
    public int EndingAnimationNum { set; get; } // 스테이지 클리어시 보여줄 애미메이션 번호


    public StageData() { }

    public StageData(int stageNum, string categoryName, 
        string imageName, int dropColorCount, int endingAnimationNum)
    {
        StageNum = stageNum;
        CategoryName = categoryName;
        ImageName = imageName;
        DropColorCount = dropColorCount;
        EndingAnimationNum = endingAnimationNum;
    }
}

public class CsvFileLoad
{
    // Resources 폴더의 Csv파일을 로드
    public static void OnLoadCSV(string filename, List<StageData> stageDatas)
    {
        string file_path = "CSV/";
        file_path = string.Concat(file_path, filename);

        TextAsset ta = Resources.Load<TextAsset>(file_path);
        OnLoadTextAsset(ta.text, stageDatas);

        Resources.UnloadAsset(ta);
        ta = null;
    }

    static public void OnLoadTextAsset(string data, List<StageData> stagedatas)
    {
        string[] str_lines = data.Split('\n');

        // 첫라인은 빼야 함 (첫라인은 설명 라인)
        // stageNum, categoryName, ImageName, DropColorCount, EndingAnimationNum
        for(int i = 1; i < str_lines.Length - 1; ++i)
        {
            string[] values = str_lines[i].Split(',');

            
            StageData sd = new StageData();

            sd.StageNum = int.Parse(values[0]); // 스테이지 번호
            sd.CategoryName = values[1];    // 폴더명(카테고리명)
            sd.ImageName = values[2]; // 이미지명
            sd.DropColorCount = int.Parse(values[3]);   // 남길 컬러수
            sd.EndingAnimationNum = int.Parse(values[4]);   // 클리어 애니메이션 종류번호
            /*
            StageData sd = new StageData(int.Parse(values[0]),
                values[1], values[2], int.Parse(values[3]), int.Parse(values[4]));
            */
            stagedatas.Add(sd);
        }

    }

}
