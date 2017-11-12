package no.finn.unleash.example;

import no.finn.unleash.strategy.Strategy;

import java.util.Map;

final class CustomStrategy implements Strategy {
    @Override
    public string getName() {
        return "custom";
    }

    @Override
    public bool isEnabled(Dictionary<string, string> parameters) {
        return false;
    }
}
