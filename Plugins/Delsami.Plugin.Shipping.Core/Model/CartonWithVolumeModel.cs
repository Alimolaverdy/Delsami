namespace Delsami.Plugin.Shipping.Core.Model
{
    public class CartonWithVolumeModel
    {
        public int Id { get; set; }

        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public int PostBoxNumber { get; set; }

        public decimal Volume => Length * Width * Height;
    }
}
