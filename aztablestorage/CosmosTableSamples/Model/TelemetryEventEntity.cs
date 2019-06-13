namespace CosmosTableSamples.Model
{
    using Microsoft.Azure.Cosmos.Table;
    using System;
    using System.Text;

    public class TelemetryEventEntity : TableEntity
    {
        public string Timestamp { get; set; }

        public string EventId { get; set; }

        public string SomeData { get; set; }

        public TelemetryEventEntity()
        {
        }

        public TelemetryEventEntity(string carId, string timeStamp, string msgId)
        {
            PartitionKey = carId;
            this.Timestamp = timeStamp;
            this.EventId = msgId;

            RowKey = $"{timeStamp}-{msgId}";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                sb.Append("0000000000");
            }

            this.SomeData = sb.ToString();
        }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
