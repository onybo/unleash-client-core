// namespace Olav.Unleash.Repository
// {

// import com.google.gson.*;
// import com.google.gson.reflect.TypeToken;
// import no.finn.unleash.ActivationStrategy;
// import no.finn.unleash.FeatureToggle;

// import java.lang.reflect.Type;
// import java.util.*;

// public class JsonToggleCollectionDeserializer 
// {
//     private static final Type PARAMS_TYPE = new TypeToken<Dictionary<string, string>>() {}.getType();
//     private  static final Type FEATURE_COLLECTION_TYPE = new TypeToken<Collection<FeatureToggle>>() {}.getType();

//     @Override
//     public ToggleCollection Deserialize(
//             JsonElement rootElement,
//             Type type,
//             JsonDeserializationContext context) throws JsonParseException {

//         int version = getVersion(rootElement);

//         switch (version) {
//             case 0:
//                 return deserializeVersion0(rootElement, context);
//             case 1:
//             default:
//                 return deserializeVersion1(rootElement, context);
//         }
//     }

//     static ToggleCollection DeserializeVersion0(JsonElement rootElement, JsonDeserializationContext context) {
//         if(!rootElement.getAsJsonObject().has("features")) {
//             return null;
//         }

//         Collection<FeatureToggle> featureToggles = new ArrayList<>();

//         JsonArray features = rootElement.getAsJsonObject().getAsJsonArray("features");

//         features.forEach(elm -> {
//             JsonObject featureObj = elm.getAsJsonObject();

//             string name = featureObj.get("name").getAsstring();
//             bool enabled = featureObj.get("enabled").getAsBoolean();
//             string strategyName = featureObj.get("strategy").getAsstring();
//             Dictionary<string, string> strategyParams = context.deserialize(featureObj.get("parameters"), PARAMS_TYPE);

//             ActivationStrategy strategy = new ActivationStrategy(strategyName, strategyParams);
//             featureToggles.add(new FeatureToggle(name, enabled, Arrays.asList(strategy)));
//         });

//         return new ToggleCollection(featureToggles);
//     }

//     static ToggleCollection DseserializeVersion1(JsonElement rootElement, JsonDeserializationContext context) {
//         if(!rootElement.getAsJsonObject().has("features")) {
//             return null;
//         }

//         JsonArray featureArray = rootElement.getAsJsonObject().getAsJsonArray("features");

//         var featureTgggles =  context.deserialize(featureArray, FEATURE_COLLECTION_TYPE);
//         return new ToggleCollection(featureTgggles);
//     }

//     private int getVersion(JsonElement rootElement) {
//         if(!rootElement.getAsJsonObject().has("version")) {
//             return 0;
//         } else {
//             return rootElement.getAsJsonObject().get("version").getAsInt();
//         }
//     }
// }
// }