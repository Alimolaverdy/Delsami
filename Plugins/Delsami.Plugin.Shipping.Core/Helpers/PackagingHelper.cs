using Delsami.Plugin.Shipping.Core.Domain;
using Delsami.Plugin.Shipping.Core.Model;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delsami.Plugin.Shipping.Core.Helpers
{
    public class PackagingHelper
    {

        public static List<PackingResultModel> PackProducts(IPagedList<Carton> availableCartons, IList<Product> products)
        {
            var cartonsWithVolume = availableCartons.Select(s => new CartonWithVolumeModel
            {
                Height = s.Height,
                Width = s.Width,
                Id = s.Id,
                Length = s.Length,
                PostBoxNumber = s.PostBoxNumber,
                

            }).ToList();

            var productsWithVolume = products.Select(s => new ProductWithVolumeModel
            {
                Height = s.Height,
                Width = s.Width,
                Id = s.Id,
                Length = s.Length
            }).ToList();

            var results = new List<PackingResultModel>();
            var sortedCartons = cartonsWithVolume.OrderBy(c => c.Volume).ToList();

            foreach (var product in productsWithVolume)
            {
                bool packed = false;

                // سعی می‌کنیم این محصول رو توی یکی از کارتن‌های باز شده جا بدیم
                foreach (var result in results)
                {
                    if (CanFit(result.Carton, product, result.Products))
                    {
                        result.Products.Add(product);
                        packed = true;
                        break;
                    }
                }

                // اگر توی هیچ کارتن باز شده جا نشد
                if (!packed)
                {
                    // کوچک‌ترین کارتن ممکن پیدا کن
                    var carton = sortedCartons.FirstOrDefault(c =>
                        c.Length >= product.Length &&
                        c.Width >= product.Width &&
                        c.Height >= product.Height);

                    if(carton is null)
                    {
                        carton = new CartonWithVolumeModel
                        {
                            Height = 300,
                            Length = 300,
                            PostBoxNumber = 0,
                            Width = 300,
                            Id = 0
                        };
                    }
                    var newResult = new PackingResultModel
                    {
                        Carton = new CartonWithVolumeModel
                        {
                            Id = carton.Id,
                            Length = carton.Length,
                            Width = carton.Width,
                            Height = carton.Height,
                            PostBoxNumber=carton.PostBoxNumber,
                            
                        }
                    };
                    newResult.Products.Add(product);
                    results.Add(newResult);
                }
            }

            return results;
        }

        private static bool CanFit(CartonWithVolumeModel carton, ProductWithVolumeModel product, List<ProductWithVolumeModel> products)
        {
            // چک کردن حجم
            decimal usedVolume = products.Sum(p => p.Volume);
            if (usedVolume + product.Volume > carton.Volume) return false;

            // چک کردن بزرگ‌ترین ابعاد
            decimal maxLength = Math.Max(product.Length, products.Any() ? products.Max(p => p.Length) : 0);
            decimal maxWidth = Math.Max(product.Width, products.Any() ? products.Max(p => p.Width) : 0);
            decimal maxHeight = Math.Max(product.Height, products.Any() ? products.Max(p => p.Height) : 0);

            return carton.Length >= maxLength &&
                    carton.Width >= maxWidth &&
                    carton.Height >= maxHeight;
        }
    }
}
