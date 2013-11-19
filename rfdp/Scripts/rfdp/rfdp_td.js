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



    if (mode == "TimeDomain") {
        //draw_chan_svg();
        plot_time_domain();
    } else if (mode == "Scatter") {
        plot_scatter();
    } else if (mode == "Histogram") {
        plot_histogram();
    }

});

function draw_chan_svg() {
    var data_ch0 = JSONData_ch0.slice();
    var data_ch1 = JSONData_ch1.slice();

    var svg = d3.select("#chart svg")
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

    var lineGraph_ch0 = svg.append("path")
        .attr("d", lineFunction(data_ch0))
        .attr("data-legend", "channel 0")
        .attr("stroke", "blue")
        .attr("stroke-width", 1)
        .attr("fill", "none");

    var lineGraph_ch1 = svg.append("path")
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

    var xAxisGroup = svg.append("g")
        .attr("transform", "translate(0," + (height - padding) + ")")
        .attr("class", "axis")
        .call(xAxis);

    var xAxisGroup = svg.append("g")
        .attr("transform", "translate(" + padding + ",0)")
        .attr("class", "axis")
        .call(yAxis);

    svg.append("svg:text")
        .attr("class", "title")
	    .attr("x", 20)
	    .attr("y", 20)
	    .text("Time-Domain Analyzer");


    var legend = svg.append("g")
       .attr("class", "legend")
       .attr("transform", "translate(" + (width - 80) + ", 20)")
       .style("font-size", "12px")
       .call(d3.legend);

}

function plot_scatter() {
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

    var svg = d3.select("#chart svg")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .style("border", "1px solid black");

    svg.selectAll(".dot")
      .data(data)
      .enter().append("circle")
      .attr("class", "dot")
      .attr("r", 1)
      .attr("cx", function (d) { return linearScaleX(d.x); })
      .attr("cy", function (d) { return linearScaleY(d.y); })
      .style("fill", function (d) { return "blue"; });


    var xAxis = d3.svg.axis()
    .scale(linearScaleX)
    .orient("bottom");

    var yAxis = d3.svg.axis()
        .scale(linearScaleY)
        .orient("left");

    var xAxisGroup = svg.append("g")
        .attr("transform", "translate(0," + (height - padding) + ")")
        .attr("class", "axis")
        .call(xAxis);

    var xAxisGroup = svg.append("g")
        .attr("transform", "translate(" + padding + ",0)")
        .attr("class", "axis")
        .call(yAxis);
}


function plot_time_domain() {
    nv.addGraph(function () {
        chart = nv.models.lineChart()
        .options({
            margin: { left: 100, bottom: 100 },
            x: function (d, i) { return i },
            showXAxis: true,
            showYAxis: true,
            transitionDuration: 0
        })
        ;

        // chart sub-models (ie. xAxis, yAxis, etc) when accessed directly, return themselves, not the parent chart, so need to chain separately
        chart.xAxis
          .axisLabel("Time (ms)")
          .tickFormat(d3.format(',.4f'));

        chart.yAxis
          .axisLabel('Voltage (v)')
          .tickFormat(d3.format(',.2f'))
        ;

        d3.select('#chart svg')
          .datum(sinAndCos())
          .call(chart);

        //TODO: Figure out a good way to do this automatically
        nv.utils.windowResize(chart.update);
        //nv.utils.windowResize(function() { d3.select('#chart1 svg').call(chart) });

        chart.dispatch.on('stateChange', function (e) { nv.log('New State:', JSON.stringify(e)); });

        return chart;
    });

    function sinAndCos() {
        var data_ch0 = JSONData_ch0.slice();
        var data_ch1 = JSONData_ch1.slice();
        var sin = [],
          cos = []
        ;
        
        var len = Object.keys(data_ch0).length > Object.keys(data_ch1).length ? Object.keys(data_ch1).length : Object.keys(data_ch0).length;
        for (var i = 0; i < len; i++) {
            sin.push({ x: data_ch0[i].x, y: data_ch0[i].y });
            cos.push({ x: data_ch1[i].x, y: data_ch1[i].y });
        }

        return [
          {
              values: sin,
              key: "Channel 0",
              color: "blue"
          },
        {
            values: cos,
            key: "Channel 1",
            color: "red"
        }
        ];
    }
}

function plot_histogram() {
    var data_hist = JSONData_hist.slice();
    nv.addGraph(function () {
        var chart = nv.models.discreteBarChart()
               .x(function (d) { return d.label })
               .y(function (d) { return d.value })
               .staggerLabels(true)
               .tooltips(false)
               .showValues(true)

        d3.select('#chart svg')
            .datum(data_hist)
          .transition().duration(500)
            .call(chart);

        nv.utils.windowResize(chart.update);

        return chart;
    });

}