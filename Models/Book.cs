﻿namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int? PublishedYear { get; set; }
        public string ISBN { get; set; }
        public decimal? Price { get; set; }
    }
}