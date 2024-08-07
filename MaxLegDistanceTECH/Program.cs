using Google.OrTools.ConstraintSolver;

const long thresholdDistance = 200;
const string distanceDimensionName = "Distance";

DataModel data = new();

RoutingIndexManager manager = new(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);
RoutingModel routing = new(manager);

var transitCallBackIndex = routing.RegisterTransitCallback((fromIndex, toIndex) =>
{
    var fromNode = manager.IndexToNode(fromIndex);
    var toNode = manager.IndexToNode(toIndex);

    return data.DistanceMatrix[fromNode, toNode];
});

routing.SetArcCostEvaluatorOfAllVehicles(transitCallBackIndex);
routing.AddDimension(transitCallBackIndex, 0, long.MaxValue, true, distanceDimensionName);

var distanceDimension = routing.GetMutableDimension(distanceDimensionName);

var nodesCount = data.DistanceMatrix.GetLength(0);
var solver = routing.solver();

for (var from = 0; from < nodesCount; from++)
{
    for (var to = 0; to < nodesCount; to++)
    {
        if (from != to)
        {
            var distance = data.DistanceMatrix[from, to];
            if (distance > thresholdDistance)
            {
                var fromIndex = manager.NodeToIndex(from);
                var toIndex = manager.NodeToIndex(to);

                solver.Add(solver.MakeLessOrEqual(solver.MakeDifference(distanceDimension.CumulVar(fromIndex), distanceDimension.CumulVar(toIndex)), thresholdDistance));
            }
        }
    }
}

var searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
searchParameters.LogSearch = true;

var solution = routing.SolveWithParameters(searchParameters);

PrintSolution(data, routing, manager, solution);

return;

static void PrintSolution(in DataModel data, in RoutingModel routing, in RoutingIndexManager manager, in Assignment? solution)
{
    if (solution is null)
    {
        Console.WriteLine("No solution found!");
        return;
    }

    Console.WriteLine($"Objective {solution.ObjectiveValue()}:");

    long maxRouteDistance = 0;
    long totalDistance = 0;

    for (var i = 0; i < data.VehicleNumber; ++i)
    {
        Console.WriteLine("Route for Vehicle {0}:", i);
        long routeDistance = 0;
        var index = routing.Start(i);
        while (routing.IsEnd(index) is false)
        {
            Console.Write("{0} -> ", manager.IndexToNode((int)index));
            var previousIndex = index;
            index = solution.Value(routing.NextVar(index));
            routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
        }
        Console.WriteLine("{0}", manager.IndexToNode((int)index));
        Console.WriteLine("Distance of the route: {0}m", routeDistance);
        maxRouteDistance = Math.Max(routeDistance, maxRouteDistance);
        totalDistance += maxRouteDistance;
    }
    Console.WriteLine("Maximum distance of the routes: {0}m", maxRouteDistance);
    Console.WriteLine("Total distance of the routes: {0}m", totalDistance);
}

internal class DataModel
{
    public long[,] DistanceMatrix = {
        { 0, 300, 50 },
        { 300, 0, 60 },
        { 50, 60, 0 }
    };

    public int VehicleNumber = 2;
    public int Depot = 0;
}