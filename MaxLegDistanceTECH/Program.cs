using Google.OrTools.ConstraintSolver;

const long thresholdDistance = 200L;
const string distanceDimensionName = "Distance";

DataModel data = new();

RoutingIndexManager manager = new(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);
RoutingModel routing = new(manager);

int transitCallBackIndex = routing.RegisterTransitCallback((fromIndex, toIndex) =>
{
    var fromNode = manager.IndexToNode(fromIndex);
    var toNode = manager.IndexToNode(toIndex);

    return data.DistanceMatrix[fromNode, toNode];
});

routing.SetArcCostEvaluatorOfAllVehicles(transitCallBackIndex);
routing.AddDimension(transitCallBackIndex, 0, long.MaxValue, true, distanceDimensionName);

RoutingDimension distanceDimension = routing.GetMutableDimension(distanceDimensionName);
Solver solver = routing.solver();

for (int i = 0; i < data.VehicleNumber; i++)
{
    long index = routing.End(i);
    Console.WriteLine(manager.IndexToNode(index));
    IntVar? indexVar = distanceDimension.CumulVar(index);
    solver.Add(solver.MakeLessOrEqual(indexVar, thresholdDistance));
    // Another solution: distanceDimension.CumulVar(index).SetMax(thresholdDistance);
}

RoutingSearchParameters searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
searchParameters.LogSearch = true;

Assignment solution = routing.SolveWithParameters(searchParameters);

for (int i = 0; i < data.VehicleNumber; i += 1)
{
    long endIndex = routing.End(i);
    var endIndexVar = distanceDimension.CumulVar(endIndex).Var();
}

PrintSolution(data, routing, manager, solution);

return;

static void PrintSolution(in DataModel data, in RoutingModel routing, in RoutingIndexManager manager,
    in Assignment? solution)
{
    if (solution is null)
    {
        Console.WriteLine("No solution found!");
        return;
    }

    Console.WriteLine($"Objective {solution.ObjectiveValue()}:");

    long maxRouteDistance = 0;
    long totalDistance = 0;

    for (int i = 0; i < data.VehicleNumber; ++i)
    {
        Console.WriteLine("Route for Vehicle {0}:", i);
        long routeDistance = 0;
        var index = routing.Start(i);
        while (routing.IsEnd(index) == false)
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
        { 0, 270, 90 },
        { 270, 0, 150 },
        { 90, 150, 0 }
    };

    public int VehicleNumber = 2;
    public int Depot = 0;
};