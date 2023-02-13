using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Reflection;
using WebTruss.DynamoDB.Attributes;
using WebTruss.DynamoDB.Pagination;
using WebTruss.Exception;

namespace WebTruss.DynamoDB
{
    public class DynamoSet<T>
    {
        private readonly AmazonDynamoDBClient client;
        private readonly string tableName;
        public DynamoSet(DynamoContext context)
        {
            KeyValuePair<Type, string>? table = context.Tables.Where(x => x.Key == typeof(T)).FirstOrDefault();

            if (table == null)
            {
                throw new System.Exception($"No table name found for type of {nameof(T)}");
            }

            if (!typeof(T).GetProperties().Any())
            {
                throw new System.Exception($"Model of type {nameof(T)} does not have any properties");
            }

            tableName = table.Value.Value;
            client = context.Client;
        }

        public async Task<bool> AddAsync(T data)
        {
            var item = new Dictionary<string, AttributeValue>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(data);
                if (value == null)
                {
                    continue;
                }
                item.Add(property.Name, new AttributeValue(value.ToString()));
            }
            var request = new PutItemRequest
            {
                TableName = tableName,
                Item = item
            };
            var response = await client.PutItemAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> DeleteAsync(string pk)
        {
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            var pkProperty = this.GetPkName();
            if (pkProperty == null)
            {
                throw new System.Exception($"Pk not found for entity {nameof(T)}");
            }
            key.Add(pkProperty.Name, new AttributeValue(pk));
            return await this.DeleteAsync(key);
        }

        public async Task<bool> DeleteAsync(string pk, string sk)
        {
            Dictionary<string, AttributeValue> keys = new Dictionary<string, AttributeValue>();
            var pkProperty = this.GetPkName();
            if (pkProperty == null)
            {
                throw new System.Exception($"Pk not found for entity {nameof(T)}");
            }
            var skProperty = this.GetSkName();
            if (skProperty == null)
            {
                throw new System.Exception($"Sk not found for entity {nameof(T)}");
            }
            keys.Add(pkProperty.Name, new AttributeValue(pk));
            keys.Add(skProperty.Name, new AttributeValue(sk));
            return await this.DeleteAsync(keys);
        }

        public async Task<bool> DeleteAsync(Dictionary<string, AttributeValue> keys)
        {
            var request = new DeleteItemRequest
            {
                TableName = tableName,
                Key = keys
            };
            var response = await client.DeleteItemAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<T?> GetByPrimaryKeyAsync(string pk)
        {
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            var pkProperty = this.GetPkName();
            if (pkProperty == null)
            {
                throw new System.Exception($"Pk not found for entity {nameof(T)}");
            }
            key.Add(pkProperty.Name, new AttributeValue(pk));
            return await this.GetByKeysAsync(key);
        }

        public async Task<T?> GetByKeysAsync(string pk, string sk)
        {
            Dictionary<string, AttributeValue> keys = new Dictionary<string, AttributeValue>();
            var pkProperty = this.GetPkName();
            if (pkProperty == null)
            {
                throw new System.Exception($"Pk not found for entity {nameof(T)}");
            }

            var skProperty = this.GetSkName();
            if (skProperty == null)
            {
                throw new System.Exception($"Sk not found for entity {nameof(T)}");
            }
            keys.Add(pkProperty.Name, new AttributeValue(pk));
            keys.Add(skProperty.Name, new AttributeValue(sk));
            return await this.GetByKeysAsync(keys);
        }

        public async Task<T?> GetByKeysAsync(Dictionary<string, AttributeValue> keys)
        {
            var request = new GetItemRequest
            {
                TableName = tableName,
                Key = keys
            };

            var response = await client.GetItemAsync(request);
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return default(T);
            }

            var data = (T)Activator.CreateInstance(typeof(T));
            foreach (var property in typeof(T).GetProperties())
            {
                var value = Activator.CreateInstance(property.GetType());
                if (!response.Item.Where(x => x.Key == property.Name).Any())
                {
                    continue;
                }
                value = response.Item.Where(x => x.Key == property.Name).FirstOrDefault().Value;
                property.SetValue(data, value);
            }
            return data;
        }

        public async Task<PagedDynamoResult<T>> GetListAsync(string? paginationToken, int limit, QueryFilter? filter = null)
        {
            var table = Table.LoadTable(client, tableName);
            var config = new QueryOperationConfig
            {
                Limit = limit,
                Select = SelectValues.AllAttributes,
                ConsistentRead = true,
                Filter = filter,
                PaginationToken = paginationToken
            };

            Search search = table.Query(config);
            var searchResult = await search.GetNextSetAsync();

            var result = new PagedDynamoResult<T>();
            result.PaginationToken = search.PaginationToken;
            foreach (var item in searchResult)
            {
                var data = (T)Activator.CreateInstance(typeof(T));
                foreach (var property in typeof(T).GetProperties())
                {
                    var value = Activator.CreateInstance(property.GetType());
                    if (item.Where(x => x.Key == property.Name).Any())
                    {
                        continue;
                    }
                    value = item.Where(x => x.Key == property.Name).FirstOrDefault().Value;
                    property.SetValue(data, value);
                }
                result.Items.Add(data);
            }
            return result;
        }

        public async Task<bool> UpdateAsync(T data)
        {
            Dictionary<string, AttributeValue> keys = new Dictionary<string, AttributeValue>();
            var pkProperty = this.GetPkName();
            if (pkProperty == null)
            {
                throw new System.Exception($"Pk not found for entity {nameof(T)}");
            }
            keys.Add(pkProperty.Name, new AttributeValue(pkProperty.GetValue(data).ToString()));

            var skProperty = this.GetSkName();
            if (skProperty != null)
            {
                keys.Add(skProperty.Name, new AttributeValue(skProperty.GetValue(data).ToString()));
            }

            string updateExpression = "set ";
            var expressionAttributeNames = new Dictionary<string, string>();
            var expressionAttributeValues = new Dictionary<string, AttributeValue>();
            foreach (var property in typeof(T).GetProperties())
            {
                updateExpression += $"#{property.Name}=:{property.Name}, ";
                expressionAttributeNames.Add($"#{property.Name}", property.Name);
                expressionAttributeValues.Add($":{property.Name}", new AttributeValue(property.GetValue(data).ToString()));
            }
            var request = new UpdateItemRequest
            {
                TableName = tableName,
                Key = keys,
                UpdateExpression = updateExpression,
                ExpressionAttributeNames = expressionAttributeNames,
                ExpressionAttributeValues = expressionAttributeValues
            };
            var response = await client.UpdateItemAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        private PropertyInfo? GetPkName()
        {
            return typeof(T).GetProperties().Where(x => x.CustomAttributes.Where(x => x is Pk).Any()).FirstOrDefault();
        }

        private PropertyInfo? GetSkName()
        {
            return typeof(T).GetProperties().Where(x => x.CustomAttributes.Where(x => x is Sk).Any()).FirstOrDefault();
        }
    }
}
