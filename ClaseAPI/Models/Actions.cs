﻿namespace ClaseAPI.Models
{
    public class Actions
    {
        public int TxNumber {  get; set; }
        public DateTime OrderDate { get; set; }
        public string Action {  get; set; }
        public string Status { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceTotal { get; set; }
    }
}
