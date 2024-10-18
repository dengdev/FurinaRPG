using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Newtonsoft.json数据反序列化转换器
/// </summary>
/// <typeparam name="T">类型参数</typeparam>
public abstract class DataCreationConverter<T> : JsonConverter {
    /// <summary>
    /// 根据 JSON 对象中的属性创建对象实例
    /// </summary>
    /// <param name="objectType">预期的对象类型</param>
    /// <param name="jObject">将要反序列化的 JSON 对象内容</param>
    /// <returns>创建的对象实例</returns>
    protected abstract T Create(Type objectType, JObject jObject);

    public override bool CanConvert(Type objectType) {
        // 检查 objectType 是否可以转换为类型 T
        return typeof(T).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        // 从流中加载 JObject
        JObject jObject = JObject.Load(reader);

        // 基于 JObject 创建目标对象
        T target = Create(objectType, jObject);

        // 填充对象属性
        serializer.Populate(jObject.CreateReader(), target);

        return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        throw new NotImplementedException();// 暂未实现序列化方法
    }

    protected bool FieldExists(JObject nJObject, string nPropertyName) {
        // 检查指定属性在 JObject 中是否存在
        return nJObject[nPropertyName] != null;
    }

    protected bool FieldExists(string nFieldName, JObject nJObject, out string nEntityVel) {
        // 检查指定字段是否存在，并返回其值
        nEntityVel = nJObject[nFieldName] == null ? "" : nJObject[nFieldName].ToString();
        return nJObject[nFieldName] != null;
    }
}