﻿using System.Text.Json.Serialization;

namespace WebApi.Models.DataModels
{
    public class ProductUpdateDto
    {
        [JsonConstructor]
        public ProductUpdateDto() { }

        public ProductUpdateDto(Product product)
        {
            Name = product.Name;
            WeightInGrams = product.WeightInGrams;
            CarbonOutputPerGram = product.CarbonOutputPerGram;
            AssignedProjects = product.AssignedProjects.Select(project => project.Id).ToList();
        }

        public string Name { get; set; } = string.Empty;

        public double WeightInGrams { get; set; } = 0.0;

        public double CarbonOutputPerGram { get; set; } = 0.0;

        public List<Guid> AssignedProjects { get; set; } = new List<Guid>();
    }
}
