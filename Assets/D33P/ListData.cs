using System.Collections.Generic;

[System.Serializable]
  public class ListData
  {
    public float quartile1 { get; set; }
    public float quartile3 { get; set; }
    public float firstA { get; set; }
    public float firstB { get; set; }
    public float midRange { get; set; }
    public float artificialTop { get; set; }
    public float artificialBottom { get; set; }
    public float artificialRange { get; set; }
    public List<float> clippedAskList { get; set; }
    public List<float> clippedBidList { get; set; }

}