var width = 900,
    height = 600,
    padding = 40;

$(document).ready(function () {
    
    $("form input:radio[name=SelChan]").click(function () {
        if ($("form input:radio[name=SelChan]:checked").val() == "1") {
            $("#datalist1").attr("disabled", true);
            $("form input:radio[name=SelPanel][value=scatter_analysis]").attr("disabled", true);
        } else {
            $("#datalist1").attr("disabled", false);
            $("form input:radio[name=SelPanel][value=scatter_analysis]").attr("disabled", false);
        }
    });

    if (mode == "tdom_analysis") {
        draw_chan_svg();
    } else if (mode == "scatter_analysis") {
        draw_plan_svg();
    }
    
});

function draw_chan_svg () {
    var data_ch0 = JSONData_ch0.slice();
    var data_ch1 = JSONData_ch1.slice();

    var svgContainer = d3.select("#svg")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .style("border", "1px solid black");


    var linearScaleX = d3.scale.linear()
        .range([padding, width - padding])
        .domain(d3.extent(data_ch0, function (d) { return d.x; }));

    var linearScaleY = d3.scale.linear()
        .range([height - padding, padding])
        .domain(d3.extent(data_ch0, function (d) { return d.y; }));


    var lineFunction = d3.svg.line()
        .x(function (d) { return linearScaleX(d.x); })
        .y(function (d) { return linearScaleY(d.y); })
        .interpolate("linear");

    var lineGraph_ch0 = svgContainer.append("path")
        .attr("d", lineFunction(data_ch0))
        .attr("data-legend", "channel 0")
        .attr("stroke", "blue")
        .attr("stroke-width", 1)
        .attr("fill", "none");

    var lineGraph_ch1 = svgContainer.append("path")
        .attr("d", lineFunction(data_ch1))
        .attr("data-legend", "channel 1")
        .attr("stroke", "red")
        .attr("stroke-width", 1)
        .attr("fill", "none");

    var xAxis = d3.svg.axis()
        .scale(linearScaleX)
        .orient("bottom");

    var yAxis = d3.svg.axis()
        .scale(linearScaleY)
        .orient("left");

    var xAxisGroup = svgContainer.append("g")
        .attr("transform", "translate(0," + (height - padding) + ")")
        .attr("class", "axis")
        .call(xAxis);

    var xAxisGroup = svgContainer.append("g")
        .attr("transform", "translate(" + padding + ",0)")
        .attr("class", "axis")
        .call(yAxis);

    svgContainer.append("svg:text")
        .attr("class", "title")
	    .attr("x", 20)
	    .attr("y", 20)
	    .text("Time-Domain Analyzer");

    
    var legend = svgContainer.append("g")
       .attr("class", "legend")
       .attr("transform", "translate(" + (width - 80) + ", 20)")
       .style("font-size", "12px")
       .call(d3.legend);

}

function draw_plan_svg () {
    var data = JSONData_plan.slice();

    var linearScaleX = d3.scale.linear()
        .range([padding, width - padding])
        .domain(d3.extent(data, function (d) { return d.x; }));

    var linearScaleY = d3.scale.linear()
        .range([height - padding, padding])
        .domain(d3.extent(data, function (d) { return d.y; }));

    var xAxis = d3.svg.axis()
        .scale(linearScaleX)
        .orient("bottom");

    var yAxis = d3.svg.axis()
        .scale(linearScaleY)
        .orient("left");

    var svgContainer = d3.select("#svg")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .style("border", "1px solid black");

    svgContainer.selectAll(".dot")
      .data(data)
      .enter().append("circle")
      .attr("class", "dot")
      .attr("r", 1)
      .attr("cx", function (d) { return linearScaleX(d.x); })
      .attr("cy", function (d) { return linearScaleY(d.y); })
      .style("fill", function (d) { return color(100); });


    var xAxis = d3.svg.axis()
    .scale(linearScaleX)
    .orient("bottom");

    var yAxis = d3.svg.axis()
        .scale(linearScaleY)
        .orient("left");

    var xAxisGroup = svgContainer.append("g")
        .attr("transform", "translate(0," + (height - padding) + ")")
        .attr("class", "axis")
        .call(xAxis);

    var xAxisGroup = svgContainer.append("g")
        .attr("transform", "translate(" + padding + ",0)")
        .attr("class", "axis")
        .call(yAxis);
}