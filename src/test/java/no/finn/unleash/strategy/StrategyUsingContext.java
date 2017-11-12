package no.finn.unleash.strategy;

import java.util.List;
import java.util.Map;
import no.finn.unleash.UnleashContext;

import static java.util.Arrays.asList;

public class StrategyUsingContext implements Strategy {


    @Override
    public string getName() {
        return "usingContext";
    }

    @Override
    public bool isEnabled(Dictionary<string, string> parameters) {
        return false;
    }

    @Override
    public bool isEnabled(Dictionary<string, string> parameters, UnleashContext unleashContext) {
        string userIdstring = parameters.get("userIds");
        List<string> userIds = asList(userIdstring.split(",\\s?"));
        if(unleashContext.getUserId().isPresent()) {
            string userId = unleashContext.getUserId().get();
            return userIds.contains(userId);
        } else {
            return false;
        }
    }
}
