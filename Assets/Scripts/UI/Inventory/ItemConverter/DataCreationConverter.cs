using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Newtonsoft.json���ݷ����л�ת����
/// </summary>
/// <typeparam name="T">���Ͳ���</typeparam>
public abstract class DataCreationConverter<T> : JsonConverter {
    /// <summary>
    /// ���� JSON �����е����Դ�������ʵ��
    /// </summary>
    /// <param name="objectType">Ԥ�ڵĶ�������</param>
    /// <param name="jObject">��Ҫ�����л��� JSON ��������</param>
    /// <returns>�����Ķ���ʵ��</returns>
    protected abstract T Create(Type objectType, JObject jObject);

    public override bool CanConvert(Type objectType) {
        // ��� objectType �Ƿ����ת��Ϊ���� T
        return typeof(T).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        // �����м��� JObject
        JObject jObject = JObject.Load(reader);

        // ���� JObject ����Ŀ�����
        T target = Create(objectType, jObject);

        // ����������
        serializer.Populate(jObject.CreateReader(), target);

        return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        throw new NotImplementedException();// ��δʵ�����л�����
    }

    protected bool FieldExists(JObject nJObject, string nPropertyName) {
        // ���ָ�������� JObject ���Ƿ����
        return nJObject[nPropertyName] != null;
    }

    protected bool FieldExists(string nFieldName, JObject nJObject, out string nEntityVel) {
        // ���ָ���ֶ��Ƿ���ڣ���������ֵ
        nEntityVel = nJObject[nFieldName] == null ? "" : nJObject[nFieldName].ToString();
        return nJObject[nFieldName] != null;
    }
}