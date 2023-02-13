using Amazon.DynamoDBv2;

namespace WebTruss.DynamoDB
{
    public class DynamoContext
    {
        public AmazonDynamoDBClient Client { get; set; }
        public Dictionary<Type, string> Tables { get; set; }
    }
}
