using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;

[System.Serializable]
public class MyClass:System.Object
{
    public int date;
    public float high;
    public float low;
    public float open;
    public float close;
    public float volume;
    public float quoteVolume;
    public float weightedAverage;
}
public class CryptoChart : MonoBehaviour{
    private string httpString;
    private MyClass parsedJSON;
    private List<MyClass> parsedJSONs;
    private LineRenderer lineRenderer;


    void Start()
    {
        StartCoroutine(GetText());
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 2f;
        lineRenderer.endWidth = 2f;
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://poloniex.com/public?command=returnChartData&currencyPair=BTC_XMR&start=1405699200&end=9999999999&period=14400");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            httpString = www.downloadHandler.text.Substring(1, www.downloadHandler.text.Length - 2);
            httpString=httpString.Replace("},{", "}|{");
        
            string[] splitList= httpString.Split('|');
            lineRenderer.SetVertexCount(splitList.Length*2);
            Debug.Log("splitList:"+splitList[0]);
            //
            MyClass[] allClasses = new MyClass [splitList.Length ];
            float[] openArray= new float [splitList.Length];
            float[] closeArray = new float[splitList.Length];
            float[] highArray = new float[splitList.Length];
            float[] lowArray = new float[splitList.Length];
            float[] volumeArray = new float[splitList.Length];
            float[] quoteVolumeArray = new float[splitList.Length];
            float[] weightedAverageArray = new float[splitList.Length];
            parsedJSONs = new List<MyClass>();
            for (int i = 0; i < splitList.Length; i++)
            {
                parsedJSON = JsonUtility.FromJson<MyClass>(splitList[i]);
                openArray[i] = parsedJSON.open;
                closeArray[i] = parsedJSON.close;
                highArray[i] = parsedJSON.high;
                lowArray[i] = parsedJSON.low;
                volumeArray[i] = parsedJSON.volume;
                quoteVolumeArray[i] = parsedJSON.quoteVolume;
                weightedAverageArray[i] = parsedJSON.weightedAverage;
                //allClasses[i] = parsedJSON;
                //Debug.Log(parsedJSON.close);
                //parsedJSONs.Add(JsonUtility.FromJson<MyClass>(splitList[i]));
                
            }
            for (int i = 0; i < splitList.Length*2; i+=2)
            {
                lineRenderer.SetPosition(i, new Vector3(i / 10, openArray[i] /openArray.Max() * 1000f, 0));
                lineRenderer.SetPosition(i+1, new Vector3(i / 10, 0, 0));
            }
                //parsedJSONs.ForEach(element => Debug.Log("close is: " + element.close));
                //parsedJSONs.ForEach(element => openArray[element..SetValueDebug.Log("close is: " + element.close));
                //allClasses.
                Debug.Log(parsedJSONs);

            Debug.Log(openArray[1]);
            Debug.Log(splitList.Length);
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            Debug.Log("Results data :" + results.Length);
        }
    }
}