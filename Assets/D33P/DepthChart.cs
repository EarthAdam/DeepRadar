using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;


public class DepthChart : MonoBehaviour
{
    public string httpString;
    public LineRenderer bidLineRenderer;
    public LineRenderer askLineRenderer;
    public LineRenderer boxLine;
    private float width= 0.01f;
    private float scale = 0.4f;

    void Start()
    {
        StartCoroutine(GetText());
    }
    IEnumerator GetText()
    {
        // Web request to return JSON data of Order Book containing asks and bids
        UnityWebRequest www = UnityWebRequest.Get(httpString);
        yield return www.SendWebRequest();
        OrderBook response = JsonConvert.DeserializeObject<OrderBook>(www.downloadHandler.text);
        OrderBookMetadata block = DescribeBook(response);

        DrawAxes(block.priceBlock,block.quantBlock);
        DrawDepthChart(block);
    }
    OrderBookMetadata DescribeBook(OrderBook book1)
    {
        OrderBookMetadata meta1 = new OrderBookMetadata();
        meta1.book1 = book1;
        // Just declaring empty lists
        meta1.askPrices = new List<float>();
        meta1.askQuants = new List<float>();
        meta1.bidPrices = new List<float>();
        meta1.bidQuants = new List<float>();

        // Loop through each ASK contained in JSON response, and
        for (int i = 0;i<book1.asks.Count;i++)
        {
            meta1.askPrices.Add((float)Convert.ToDouble(book1.asks[i][0]));
            meta1.askQuants.Add((float)Convert.ToDouble(book1.asks[i][1]));
        }
        for (int i = 0;i<book1.bids.Count;i++)
        {
            meta1.bidPrices.Add((float)Convert.ToDouble(book1.bids[i][0]));
            meta1.bidQuants.Add((float)Convert.ToDouble(book1.bids[i][1]));
        }

        // Draw the Axes
        // We want to define important parameters of the Depth Chart based on the data
        // Order books usually include outliers, so we need to format the chart to show the most meaningful info
        meta1.priceBlock = DescribeLists(meta1.askPrices,meta1.bidPrices);  
        meta1.quantBlock = DescribeLists(meta1.askQuants,meta1.bidQuants);
        return meta1;
    }
    ListData DescribeLists(List<float> asks, List<float> bids)
    {
        ListData newList = new ListData();

        newList.quartile3 = asks[asks.Count()/2];
        newList.quartile1 = bids[bids.Count()/2];
        newList.midRange = newList.quartile3-newList.quartile1;
        newList.firstA = asks.First();
        newList.firstB = bids.First();
        
        newList.artificialTop = newList.quartile3 + newList.artificialRange/4;
        newList.artificialBottom = newList.quartile1 - newList.artificialRange/4;
        newList.artificialRange = newList.artificialTop - newList.artificialBottom;
        
        newList.clippedAskList = asks;
        newList.clippedBidList = bids;
        for (int i = 0; i<asks.Count();i++)
        {
            if(asks[i]>=newList.artificialTop)
                newList.clippedAskList[i] = newList.artificialTop;
        }
        for (int i = 0; i<bids.Count();i++)
        {
            if(bids[i]<newList.artificialTop)
                newList.clippedBidList[i] = newList.artificialTop;
        }
        return newList;
    }
    void DrawAxes(ListData price, ListData quant)
    {
        boxLine.useWorldSpace = false;
        boxLine.startWidth = width;
        boxLine.endWidth = width;

        boxLine.positionCount = 9;
        boxLine.SetPosition(0,new Vector3(0,0,0));
        boxLine.SetPosition(1,new Vector3(scale*2*(price.artificialTop-price.firstB)/price.artificialRange,0,0));
        boxLine.SetPosition(2,new Vector3(scale*2*(price.artificialTop-price.firstB)/price.artificialRange,0.02f,0));
        boxLine.SetPosition(3,new Vector3(scale*2*(price.artificialTop-price.firstA)/price.artificialRange,0.02f,0));
        boxLine.SetPosition(4,new Vector3(scale*2*(price.artificialTop-price.firstA)/price.artificialRange,0,0));
        boxLine.SetPosition(5,new Vector3(scale*2*(price.artificialTop-price.artificialBottom)/price.artificialRange,0,0));
        boxLine.SetPosition(6,new Vector3(scale*2*(price.artificialTop-price.artificialBottom)/price.artificialRange,scale*(quant.artificialTop-quant.artificialBottom)/quant.artificialRange,0));
        boxLine.SetPosition(7,new Vector3(0,scale*(quant.artificialTop-quant.artificialBottom)/quant.artificialRange,0));
        boxLine.SetPosition(8,new Vector3(0,0,0));
        boxLine.transform.position += new Vector3(-2*scale*(-price.firstB+price.artificialTop)/price.artificialRange,0,0);
    }
    void DrawDepthChart(OrderBookMetadata block1)
    {
        bidLineRenderer.useWorldSpace = false;
        askLineRenderer.useWorldSpace = false;
        bidLineRenderer.startWidth = width;
        bidLineRenderer.endWidth = width;
        askLineRenderer.startWidth = width;
        askLineRenderer.endWidth = width;
        askLineRenderer.positionCount = block1.book1.asks.Count;
        bidLineRenderer.positionCount = block1.book1.bids.Count; 
        // Next thing to do:
        // Sum up all of the Quantity values (remember, not in any order)
        float quantAvg = block1.askQuants.Sum()/block1.askQuants.Count() + block1.bidQuants.Sum()/block1.bidQuants.Count();
        


        // Add a few more variables for us to define later;
        float xPrice = 0.0f;
        float yQuantity = 0.0f;


        // Loop through the "asks" data. 
        // We're building a Depth Chart. This is a normal thing. Think about how Depth Charts work.
        // We want to define a Price that the vertical heights of the line will move to for each increment in price (yQuantity),
        // as well as the horizontal position of that price (xPrice)
        for (int i = 0;i<block1.book1.asks.Count;i++)
        {
            xPrice = (float)Convert.ToDouble(block1.book1.asks[i][0]);
            yQuantity += (float)Convert.ToDouble(block1.book1.asks[i][1]);
            askLineRenderer.SetPosition(i,new Vector3(scale*(xPrice-block1.priceBlock.firstA)/block1.priceBlock.artificialRange/2,scale*yQuantity/quantAvg/10,0));
        }
        //for (int i = 0;i<block1.priceBlock.clippedAskList.Count;i++)
        //{
        //    xPrice = (float)Convert.ToDouble(block1.priceBlock.clippedAskList[i]);
        //    yQuantity += (float)Convert.ToDouble(block1.quantBlock.clippedAskList[i]);
        //    askLineRenderer.SetPosition(i,new Vector3((xPrice-block1.priceBlock.firstA)/block1.priceBlock.artificialRange*2,yQuantity/block1.quantBlock.artificialRange/1000,0));
        //}
        xPrice = 0.0f;
        yQuantity = 0.0f;
        for (int i = 0;i<block1.book1.bids.Count;i++)
        {
            xPrice = (float)Convert.ToDouble(block1.book1.bids[i][0]);
            yQuantity += (float)Convert.ToDouble(block1.book1.bids[i][1]);
            bidLineRenderer.SetPosition(i,new Vector3(scale*(xPrice-block1.priceBlock.firstA)/block1.priceBlock.artificialRange/2,scale*yQuantity/quantAvg/10,0));
        }
        //askLineRenderer.transform.position += new Vector3(-2*scale*(block1.priceBlock.firstB-block1.priceBlock.artificialBottom-block1.priceBlock.artificialTop+block1.priceBlock.firstB)/block1.priceBlock.artificialRange,0,0);
        //bidLineRenderer.transform.position += new Vector3(-2*scale*(block1.priceBlock.firstB-block1.priceBlock.artificialBottom-block1.priceBlock.artificialTop+block1.priceBlock.firstB)/block1.priceBlock.artificialRange,0,0);
        xPrice = 0.0f;
        yQuantity = 0.0f;

    }

    


}