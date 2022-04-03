using System.Collections.Generic;

[System.Serializable]
  public class OrderBookMetadata
  {
    public OrderBook book1 { get; set; }
    public ListData priceBlock { get; set; }
    public ListData quantBlock { get; set; }

    public List<float> askPrices { get; set; }
    public List<float> askQuants { get; set; }
    public List<float> bidPrices { get; set; }
    public List<float> bidQuants { get; set; }
}