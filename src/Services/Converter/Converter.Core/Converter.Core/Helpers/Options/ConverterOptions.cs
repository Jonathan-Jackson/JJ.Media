namespace Converter.Core.Helpers.Options {

    public class ConverterOptions {
        public string ProcessingStore { get; set; }
        public string QueueStore { get; set; }
        public string DownloadStore { get; set; }
        public string ProcessedStore { get; set; }
    }
}