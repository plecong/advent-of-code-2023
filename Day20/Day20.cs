namespace AdventOfCode2023.Day20;

using Xunit;
using AdventOfCode2023.Utils;

internal enum Strategy
{
    BROADCASTER,
    FLIP_FLOP,
    CONJUNCTION,
    SINK,
    BUTTON
}

internal class Orchestrator(IEnumerable<string> input)
{
    private readonly Dictionary<string, Module> modules = Parse(input);
    private readonly Queue<PulseEvent> pending = new();

    public void Enqueue(PulseEvent pulse)
    {
        pending.Enqueue(pulse);
    }

    public void EnqueueRange(IEnumerable<PulseEvent> pulses)
    {
        foreach (var pulse in pulses)
        {
            Enqueue(pulse);
        }
    }

    public long PushButton(int count)
    {
        var low = 0L;
        var high = 0L;

        foreach (var index in Enumerable.Range(0, count))
        {
            var completed = PushButton();
            low += completed.Count(x => x.Pulse == Pulse.LOW);
            high += completed.Count(x => x.Pulse == Pulse.HIGH);
        }

        return low * high;
    }

    public List<PulseEvent> PushButton()
    {
        List<PulseEvent> completed = new();

        // send from the button a low pulse to broadcaster
        Enqueue(new PulseEvent("button", "broadcaster", Pulse.LOW));

        // process all pending pulses
        while (pending.TryDequeue(out var @event))
        {
            // log the event for tracking
            completed.Add(@event);

            // process the pulse
            modules[@event.Destination].Handle(@event, this);
        }

        return completed;
    }

    public static Dictionary<string, Module> Parse(IEnumerable<string> lines)
    {
        var modules = lines
            .Select(Parse)
            .ToDictionary(x => x.Name, x => x);

        var conjunctions = modules.Values
            .Where(x => x.Strategy == Strategy.CONJUNCTION)
            .Select(x => x.Name)
            .ToHashSet();

        // create unknown outputs
        var sinks = modules.Values
            .SelectMany(x => x.Outputs)
            .Distinct()
            .Where(x => !modules.ContainsKey(x))
            .ToList();

        foreach (var sink in sinks)
        {
            modules[sink] = new Sink(sink);
        }

        // wire up inputs on conjunctions
        foreach (var module in modules.Values)
        {
            foreach (var output in module.Outputs.Where(conjunctions.Contains))
            {
                if (modules[output] is Conjunction conj)
                {
                    conj.RegisterInput(module.Name);
                }
            }
        }

        return modules;
    }

    public static Module Parse(string line)
    {
        var (name, outs, _) = line.Split(" -> ");
        var outputs = outs.Split(",").Select(x => x.Trim()).ToList();

        return name switch
        {
            var x when x == "broadcaster" => new Broadcaster(name, outputs),
            var x when x.StartsWith("%") => new FlipFlop(x[1..], outputs),
            var x when x.StartsWith("&") => new Conjunction(x[1..], outputs),
            _ => new Sink(name)
        };
    }
}

internal enum Pulse
{
    HIGH,
    LOW
}

internal record PulseEvent(string Source, string Destination, Pulse Pulse)
{
    public override string ToString()
    {
        var pulse = Pulse == Pulse.HIGH ? "high" : "low";
        return $"{Source} -{pulse}-> {Destination}";
    }
}

internal interface Module
{
    string Name { get; }
    Strategy Strategy { get; }

    List<string> Outputs { get; }

    /// <summary>
    /// Takes a pulse event and if activated enqueues a new
    /// pulse as necessary to the orchestrator.
    /// </summary>
    /// <param name="pulse"></param>
    /// <param name="orchestrator"></param>
    void Handle(PulseEvent evt, Orchestrator orchestrator);
}

internal class Broadcaster(string name, List<string> outputs) : Module
{
    public string Name { get; init; } = name;
    public Strategy Strategy { get; } = Strategy.BROADCASTER;
    public List<string> Outputs { get; init; } = outputs;

    public void Handle(PulseEvent evt, Orchestrator orchestrator)
    {
        orchestrator.EnqueueRange(Outputs.Select(x => new PulseEvent(Name, x, evt.Pulse)));
    }
}

internal class FlipFlop(string name, List<string> outputs) : Module
{
    public string Name { get; init; } = name;
    public Strategy Strategy { get; } = Strategy.FLIP_FLOP;
    public List<string> Outputs { get; init; } = outputs;
    public bool On { get; private set; } = false;

    public void Handle(PulseEvent evt, Orchestrator orchestrator)
    {
        // ignore high
        if (evt.Pulse == Pulse.HIGH) return;

        // if low, flip the switch
        On = !On;

        // if new state is On then send High
        var pulse = On ? Pulse.HIGH : Pulse.LOW;

        orchestrator.EnqueueRange(Outputs.Select(x => new PulseEvent(Name, x, pulse)));
    }
}

internal class Conjunction(string name, List<string> outputs) : Module
{
    public string Name { get; init; } = name;
    public Strategy Strategy { get; } = Strategy.CONJUNCTION;
    public List<string> Outputs { get; init; } = outputs;
    private readonly Dictionary<string, Pulse> inputs = new();

    public void RegisterInput(string name)
    {
        inputs[name] = Pulse.LOW;
    }

    public void Handle(PulseEvent evt, Orchestrator orchestrator)
    {
        inputs[evt.Source] = evt.Pulse;

        // if all inputs are high, send low else high
        var pulse = inputs.Values.All(x => x == Pulse.HIGH) ? Pulse.LOW : Pulse.HIGH;

        orchestrator.EnqueueRange(Outputs.Select(x => new PulseEvent(Name, x, pulse)));
    }
}

internal class Sink(string name) : Module
{
    public string Name { get; init; } = name;
    public Strategy Strategy { get; } = Strategy.SINK;
    public List<string> Outputs { get; init; } = [];
    public void Handle(PulseEvent evt, Orchestrator orchestrator)
    {
        // ignore
    }
}

internal class Solution()
{
    public long Part1(IEnumerable<string> input)
    {
        var orchestrator = new Orchestrator(input);
        return orchestrator.PushButton(1000);
    }

    public long Part2(IEnumerable<string> input)
    {
        return 0;
    }
}

public class Test()
{
    private readonly Solution solution = new();

    public IEnumerable<string> Sample
    {
        get => """
            broadcaster -> a, b, c
            %a -> b
            %b -> c
            %c -> inv
            &inv -> a
            """.ReadLines();
    }

    public IEnumerable<string> Sample2
    {
        get => """
            broadcaster -> a
            %a -> inv, con
            &inv -> b
            %b -> con
            &con -> output
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void SamplePushButton()
    {
        var orchestrator = new Orchestrator(Sample);
        var actual = orchestrator.PushButton().Select(x => x.ToString());
        var expected = """
            button -low-> broadcaster
            broadcaster -low-> a
            broadcaster -low-> b
            broadcaster -low-> c
            a -high-> b
            b -high-> c
            c -high-> inv
            inv -low-> a
            a -low-> b
            b -low-> c
            c -low-> inv
            inv -high-> a
            """.ReadLines().ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Sample2PushButtonOnce()
    {
        var orchestrator = new Orchestrator(Sample2);
        var actual = orchestrator.PushButton().Select(x => x.ToString());
        var expected = """
            button -low-> broadcaster
            broadcaster -low-> a
            a -high-> inv
            a -high-> con
            inv -low-> b
            con -high-> output
            b -high-> con
            con -low-> output
            """.ReadLines().ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Sample2PushButtonTwice()
    {
        var orchestrator = new Orchestrator(Sample2);
        orchestrator.PushButton();

        var actual = orchestrator.PushButton().Select(x => x.ToString());
        var expected = """
            button -low-> broadcaster
            broadcaster -low-> a
            a -low-> inv
            a -low-> con
            inv -high-> b
            con -high-> output
            """.ReadLines().ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Sample2PushButtonThirdTime()
    {
        var orchestrator = new Orchestrator(Sample2);
        orchestrator.PushButton(2);

        var actual = orchestrator.PushButton().Select(x => x.ToString());
        var expected = """
            button -low-> broadcaster
            broadcaster -low-> a
            a -high-> inv
            a -high-> con
            inv -low-> b
            con -low-> output
            b -low-> con
            con -high-> output
            """.ReadLines().ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Sample2PushButtonFourthTime()
    {
        var orchestrator = new Orchestrator(Sample2);
        orchestrator.PushButton(3);

        var actual = orchestrator.PushButton().Select(x => x.ToString());
        var expected = """
            button -low-> broadcaster
            broadcaster -low-> a
            a -low-> inv
            a -low-> con
            inv -high-> b
            con -high-> output
            """.ReadLines().ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(32000000, solution.Part1(Sample));

    [Fact]
    public void Part1Sample2() =>
        Assert.Equal(11687500, solution.Part1(Sample2));

    [Fact]
    public void Part1() =>
        Assert.Equal(867_118_762, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(0, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(0, solution.Part2(Input));
}
