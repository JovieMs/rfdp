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

    $("form input:radio[name=SelPanel]").click(function () {
        if ($("form input:radio[name=SelPanel]:checked").val() == "FrequencyDomain" ||
            $("form input:radio[name=SelPanel]:checked").val() == "Histogram") {
            $("#rb_active_chan").attr("style", "visibility:display");
        } else {
            $("#rb_active_chan").attr("style", "visibility:hidden");
        }
    });

    if (mode == "TimeDomain") {
        plot_time_domain();
    } else if (mode == "Scatter") {
        plot_scatter();
    } else if (mode == "Histogram") {
        plot_histogram();
    } else if (mode == "FrequencyDomain") {
        plot_spectrum();
    }

});

function plot_scatter() {
    var data = JSONData_scatter.slice();

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

function plot_spectrum() {
    
    var chart;
    nv.addGraph(function () {
        chart = nv.models.historicalBarChart();
        chart
          .margin({ left: 100, bottom: 100 })
          .x(function (d, i) { return i })
          .transitionDuration(250)
        ;

        // chart sub-models (ie. xAxis, yAxis, etc) when accessed directly, return themselves, not the parent chart, so need to chain separately
        chart.xAxis
          .axisLabel("Frequency (MHz)")
          .tickFormat(d3.format(',.1f'));

        chart.yAxis
          .axisLabel('Magnitude')
          .tickFormat(d3.format(',.2f'));

        chart.showXAxis(true);

        d3.select('#chart svg')
          .datum(sinData())
          .transition().duration(0)
          .call(chart);

        //TODO: Figure out a good way to do this automatically
        nv.utils.windowResize(chart.update);
        //nv.utils.windowResize(function() { d3.select('#chart1 svg').call(chart) });

        chart.dispatch.on('stateChange', function (e) { nv.log('New State:', JSON.stringify(e)); });

        return chart;
    });


    function sinData() {
        var data_spec = JSONData_spec.slice();
        var sin = [];

        for (var i = 0; i < data_spec.length; i++) {
            sin.push({ x: data_spec[i].x, y: data_spec[i].y });
        }

        return [
          {
              values: sin,
              key: "Frequency Spectrum",
              color: "#ff7f0e"
          }
        ];
    }

}