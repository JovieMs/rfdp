(function () {

    //JSONData = [ { "x": 1,   "y": 5},  { "x": 20,  "y": 20},
    //              { "x": 40,  "y": 10}, { "x": 60,  "y": 40},
    //              { "x": 80,  "y": 5},  { "x": 100, "y": 60}];

    
    
    alert(JSONData[0].x);

    var data = JSONData.slice();

    var width = 600,
        height = 400,
        padding = 4;

    var svgContainer = d3.select("#svg")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("padding", padding)
        .attr("margin", 4)
        .style("border", "1px solid black");



    var linearScaleX = d3.scale.linear()
        .range([0, width])
        .domain(d3.extent(data, function (d) { return d.x; }));

    var linearScaleY = d3.scale.linear()
        .range([0, height])
        .domain(d3.extent(data, function (d) { return d.y; }));


    var lineFunction = d3.svg.line()
        .x(function (d) { return linearScaleX(d.x); })
        .y(function (d) { return linearScaleY(d.y); })
        .interpolate("linear");



    var lineGraph = svgContainer.append("path")
        .attr("d", lineFunction(data))
        .attr("stroke", "blue")
        .attr("stroke-width", 2)
        .attr("fill", "none");

    var xAxis = d3.svg.axis()
    .scale(linearScaleX)
    .orient("top");

    var yAxis = d3.svg.axis()
        .scale(linearScaleY)
        .orient("left");
    
    var xAxisGroup = svgContainer.append("g")
        .attr("transform", "translate(0," + (height-padding) + ")")
        .call(xAxis);
    var xAxisGroup = svgContainer.append("g")
        .attr("transform", "translate(4,0)")
        .call(yAxis);

    //var margin = { top: 20, right: 20, bottom: 30, left: 50 },
    //    width = 600 - margin.left - margin.right,
    //    height = 400 - margin.top - margin.bottom;

    //var x = d3.time.scale()
    //    .range([0, width]);

    //var y = d3.scale.linear()
    //    .range([height, 0]);

    //var xAxis = d3.svg.axis()
    //    .scale(x)
    //    .orient("bottom");

    //var yAxis = d3.svg.axis()
    //    .scale(y)
    //    .orient("left");

    //var line = d3.svg.line()
    //    .x(function (d) { return x(d.x); })
    //    .y(function (d) { return y(d.y); });

    //var svg = d3.select("#svg")
    //    .attr("width", width + margin.left + margin.right)
    //    .attr("height", height + margin.top + margin.bottom)
    //    .append("g")
    //    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");


    //svg.append("path")
    //    .datum(data)
    //    .attr("class", "line")
    //    .attr("d", line);

})();