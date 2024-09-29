using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ItemConverter : DataCreationConverter<Item> {
    protected override Item Create(Type objectType, JObject jObject) {
        string type = jObject["item_Type"]?.ToString();

        // 根据 type 字段创建相应的子类
        return type switch {
            "Weapon" => new Weapon(),
           // "Food" => new Food(),
           // "Material" => new Material(),
            _ => new Item()
        };
    }
}