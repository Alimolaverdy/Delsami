namespace Delsami.Plugin.Shipping.Core.Model
{
    public class ProductWithVolumeModel
    {
        public int Id { get; set; }

        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Volume => Length * Width * Height;
    }
}
