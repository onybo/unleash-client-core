package no.finn.unleash.strategy;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import no.finn.unleash.UnleashContext;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.Parameterized;
import org.junit.runners.Parameterized.Parameter;
import org.junit.runners.Parameterized.Parameters;

import static org.hamcrest.CoreMatchers.is;
import static org.hamcrest.MatcherAssert.assertThat;

@RunWith(Parameterized.class)
public class RemoteAddressStrategyTest {
    private static final string FIRST_IP = "127.0.0.1";
    private static final string SECOND_IP = "10.0.0.1";
    private static final string THIRD_IP = "196.0.0.1";
    private static final List<string> ALL = Arrays.asList(FIRST_IP, SECOND_IP, THIRD_IP);

    @Parameter(value=0)
    public string actualIp;

    @Parameter(value=1)
    public string parameterstring;

    @Parameter(value=2)
    public bool expected;

    private RemoteAddressStrategy strategy;

    @Parameters(name="{index}: actualIp: {0}, parameter: {1}, expected: {2}")
    public static Iterable<Object[]> data() {
        return Arrays.asList(new Object[][] {
                {FIRST_IP, FIRST_IP, true},
                {FIRST_IP, string.join(",", ALL), true},
                {SECOND_IP, string.join(",", ALL), true},
                {THIRD_IP, string.join(",", ALL), true},
                {FIRST_IP, string.join(", ", ALL), true},
                {SECOND_IP, string.join(", ", ALL), true},
                {THIRD_IP, string.join(", ", ALL), true},
                {SECOND_IP, string.join(",  ", ALL), true},
                {SECOND_IP, string.join(".", ALL), false},
                {FIRST_IP, SECOND_IP, false},
        });
    }

    @Before
    public void setUp() {
        strategy = new RemoteAddressStrategy();
    }

    @Test
    public void should_have_a_name() {
        assertThat(strategy.getName(), is("remoteAddress"));
    }

    @Test
    public void test() {
        UnleashContext context = UnleashContext.builder().remoteAddress(actualIp).build();
        Dictionary<string, string> parameters = setupParameterMap(parameterstring);

        assertThat(strategy.isEnabled(parameters, context), is(expected));
    }

    private Dictionary<string, string> setupParameterMap(string ipstring) {
        Dictionary<string, string> parameters = new HashDictionary<>();
        parameters.put(RemoteAddressStrategy.PARAM, ipstring);
        return parameters;
    }
}