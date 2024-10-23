using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ItemConverter : DataCreationConverter<Item> {

    private string type = "type";

    protected override Item Create(Type objectType, JObject jObject) {
        string type = jObject[this.type]?.ToString();

        // ���� type �ֶδ�����Ӧ������
        return type switch {
            "Weapon" => new Weapon(),
            "Material" => new Material(),
            "Cuisine"=>new Cuisine(),
            "CultivationMaterial"=>new CultivationMaterial(),
            _ => new Item()
        };
    }
}