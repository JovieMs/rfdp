(function () {

    var data = JSONData.slice();

    var width = 600,
        height = 400,
        padding = 40;

    var svgContainer = d3.select("#svg")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .style("border", "1px solid black");



    var linearScaleX = d3.scale.linear()
        .range([padding, width - padding])
        .domain(d3.extent(data, function (d) { return d.x; }));

    var linearScaleY = d3.scale.linear()
        .range([height - padding, padding])
        .domain(d3.extent(data, function (d) { return d.y; }));


    var lineFunction = d3.svg.line()
        .x(function (d) { return linearScaleX(d.x); })
        .y(function (d) { return linearScaleY(d.y); })
        .interpolate("linear");



    var lineGraph = svgContainer.append("path")
        .attr("d", lineFunction(data))
        .attr("stroke", "blue")
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

})();

$(document).ready(function () {
    $("form input:radio[name=SelChan]").click(function ()
    {
        if ($("form input:radio[name=SelChan]:checked").val() == "Single-Channel") {
            $("#datalist1").attr("disabled", true);
            $("form input:radio[name=SelPanel][value=scatter_analysis]").attr("disabled", true);
        } else {
            $("#datalist1").attr("disabled", false);
            $("form input:radio[name=SelPanel][value=scatter_analysis]").attr("disabled", false);
        }
    });
});

