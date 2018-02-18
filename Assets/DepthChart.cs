using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;

[System.Serializable]
public class HighOrderClass:System.Object
{
    public string bids;
    public string asks;
}
public class OrderClass : System.Object
{
    public string ecSignature;
    public string exchangeContractAddress;
    public float expirationUnixTimestampSec;
    public string feeRecipient;
    public string maker;
    public float makerFee;
    public string makerTokenAddress;
    public float makerTokenAmount;
    public float salt;
    public string taker;
    public float takerFee;
    public string takerTokenAddress;
    public float takerTokenAmount;
}
public class DepthChart : MonoBehaviour
{
    private string httpString;
    private OrderClass parsedJSON;
    private LineRenderer bidLineRenderer;
    private LineRenderer askLineRenderer;
    private int width= 2;
    private int xScale = 5;
    float bidFront = 0;
    float askFront = 0;

    void Start()
    {
        StartCoroutine(GetText());
        bidLineRenderer = GameObject.Find("MakerLine").GetComponent<LineRenderer>();
        bidLineRenderer.useWorldSpace = false;
        askLineRenderer = GameObject.Find("TakerLine").GetComponent<LineRenderer>();
        askLineRenderer.useWorldSpace = false;
        bidLineRenderer.startWidth = width;
        bidLineRenderer.endWidth = width;
        askLineRenderer.startWidth = width;
        askLineRenderer.endWidth = width;
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://api.radarrelay.com/0x/v0/orderBook?baseTokenAddress=0xe41d2489571d322189246dafa5ebde1f4699f498&quoteTokenAddress=0x2956356cd2a2bf3202f771f50d3d14a367b48070");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            httpString = www.downloadHandler.text.Substring(1, www.downloadHandler.text.Length - 2);
            httpString = httpString.Replace("],", "]|");

            string[] splitList = httpString.Split('|');

            string bidBook = splitList[0].Substring(8, splitList[0].Length - 8);
            string askBook = splitList[1].Substring(8, splitList[1].Length - 8);
            Debug.Log(askBook);
            bidBook = bidBook.Replace("},{\"ecSig", "}|{\"ecSig");
            askBook = askBook.Replace("},{\"ecSig", "}|{\"ecSig");
            bidBook = bidBook.Replace("]", "\0");
            askBook = askBook.Replace(",[", "\0");
            askBook = askBook.Replace("]", "\0");
            string[] bidList = bidBook.Split('|');
            string[] askList = askBook.Split('|');
            Debug.Log("askList[0]:" + askList[0]);
            Debug.Log("askList[last]:" + askList[askList.Length-1]);
            bidLineRenderer.SetVertexCount(bidList.Length +3);
            askLineRenderer.SetVertexCount(askList.Length +3);
            float[] bidMakerArray = new float[bidList.Length];
            float[] bidTakerArray = new float[bidList.Length];
            for (int i = 0; i < bidList.Length; i++)
            {
                parsedJSON = JsonUtility.FromJson<OrderClass>(bidList[i]);
                bidMakerArray[i] = parsedJSON.makerTokenAmount;
                bidTakerArray[i] = parsedJSON.takerTokenAmount;
            }
            float[] askMakerArray = new float[askList.Length];
            float[] askTakerArray = new float[askList.Length];
            for (int i = 0; i < askList.Length; i++)
            {
                parsedJSON = JsonUtility.FromJson<OrderClass>(askList[i]);
                askMakerArray[i] = parsedJSON.makerTokenAmount;
                askTakerArray[i] = parsedJSON.takerTokenAmount;
            }
            bidLineRenderer.SetPosition(0, new Vector3(0, 0, 0));
            askLineRenderer.SetPosition(0, new Vector3(0, 0, 0));
            for (int i = 0; i < bidList.Length ; i ++)
            {
                bidFront += bidMakerArray[i] / 1000000000000000000;
                bidLineRenderer.SetPosition(i+1, new Vector3(-bidMakerArray[i] /bidTakerArray[i]*100000, bidFront, 0));
            }
            for (int i = 0; i < askList.Length; i++)
            {
                askFront += askMakerArray[i] / 10000000000000000000;
                askLineRenderer.SetPosition(i + 1, new Vector3(askMakerArray[i] / askTakerArray[i]/5 , askFront/30, 0));
            }
            bidLineRenderer.SetPosition(bidList.Length + 1, new Vector3(-bidMakerArray[bidList.Length-1]/ bidTakerArray[bidList.Length - 1] * 100000, 0, 0));
            askLineRenderer.SetPosition(askList.Length + 1, new Vector3(askMakerArray[askList.Length-1]/ askTakerArray[askList.Length - 1]/5, 0, 0));
            bidLineRenderer.SetPosition(bidList.Length + 2, new Vector3(0, 0, 0));
            askLineRenderer.SetPosition(askList.Length + 2, new Vector3(0, 0, 0));
            //
            /*
            MyClass[] allClasses = new MyClass[splitList.Length];
            float[] openArray = new float[splitList.Length];
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
            for (int i = 0; i < splitList.Length * 2; i += 2)
            {
                lineRenderer.SetPosition(i, new Vector3(i / 10, openArray[i] / openArray.Max() * 1000f, 0));
                lineRenderer.SetPosition(i + 1, new Vector3(i / 10, 0, 0));
            }
            //parsedJSONs.ForEach(element => Debug.Log("close is: " + element.close));
            //parsedJSONs.ForEach(element => openArray[element..SetValueDebug.Log("close is: " + element.close));
            //allClasses.
            Debug.Log(parsedJSONs);

            Debug.Log(openArray[1]);
            Debug.Log(splitList.Length);
            // Or retrieve results as binary data
            */
            byte[] results = www.downloadHandler.data;
            Debug.Log("Results data :"+results.Length);
        }
    }
}