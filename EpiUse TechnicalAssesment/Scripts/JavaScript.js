// ========== Tab information (No Changes) ==========
function openTab(tabName) {
    let i, tabContent, tabButtons;
    tabContent = document.getElementsByClassName("tab-content");
    for (i = 0; i < tabContent.length; i++) {
        tabContent[i].style.display = "none";
    }
    tabButtons = document.getElementsByClassName("tab-button");
    for (i = 0; i < tabButtons.length; i++) {
        tabButtons[i].className = tabButtons[i].className.replace(" active", "");
    }
    document.getElementById(tabName).style.display = "block";
    event.currentTarget.className += " active";
}

document.addEventListener("DOMContentLoaded", function () {
    const defaultTab = document.querySelector(".tab-content");
    if (defaultTab) {
        defaultTab.style.display = "block";
    }

    const defaultButton = document.querySelector(".tab-button");
    if (defaultButton) {
        defaultButton.className += " active";
    }

    if (typeof PageMethods !== 'undefined') {
        PageMethods.GetHierarchyData(
            function (response) {
                try {
                    const data = JSON.parse(response);
                    console.log("Data received:", data);
                    renderD3(data);
                } catch (e) {
                    console.error("Error:", e, "\nResponse:", response);
                    alert("Error displaying hierarchy");
                }
            },
            function (error) {
                console.error("AJAX Error:", error);
                alert("Failed to load data");
            }
        );
    } else {
        console.error("PageMethods not available");
        alert("ScriptManager configuration error");
    }
});

// ========== Main D3 Hierarchy ==========
function renderD3(rootData) {
    console.log("Rendering hierarchy with data:", rootData);

    // D3 helper functions are correctly nested inside renderD3
    function md5(string) {
        return CryptoJS.MD5(string).toString();
    }

    function getGravatarUrl(email) {
        if (!email) return "https://www.gravatar.com/avatar/?d=identicon";
        const emailHash = md5(email.trim().toLowerCase());
        return `https://www.gravatar.com/avatar/${emailHash}?d=identicon`;
    }

    const container = document.getElementById("hierarchy-container");
    if (!container) {
        console.error("Container element not found");
        return;
    }

    const width = container.clientWidth;
    const height = container.clientHeight;

    d3.select("#hierarchy-container").selectAll("*").remove();

    const treeLayout = d3.tree()
        .size([width - 100, height - 200])
        .nodeSize([100, 150]);

    const root = d3.hierarchy(rootData);
    treeLayout(root);

    const svg = d3.select("#hierarchy-container")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("viewBox", [0, 0, width, height]);

    const zoom = d3.zoom()
        .scaleExtent([0.1, 3])
        .on("zoom", (event) => {
            g.attr("transform", event.transform);
        });

    svg.call(zoom);

    const g = svg.append("g");

    const bounds = root.descendants().reduce((acc, d) => ({
        x: [Math.min(acc.x[0], d.x), Math.max(acc.x[1], d.x)],
        y: [Math.min(acc.y[0], d.y), Math.max(acc.y[1], d.y)]
    }), { x: [0, 0], y: [0, 0] });

    const dx = bounds.x[1] - bounds.x[0];
    const dy = bounds.y[1] - bounds.y[0];
    const xOffset = (width - dx) / 2 - bounds.x[0];
    const yOffset = (height - dy) / 2 - bounds.y[0];

    g.attr("transform", `translate(${xOffset},${yOffset})`);

    const tooltip = d3.select("body").append("div")
        .attr("class", "hierarchy-tooltip")
        .style("opacity", 0)
        .style("position", "absolute")
        .style("background", "#fff")
        .style("border", "1px solid #e0e0e0")
        .style("border-radius", "4px")
        .style("padding", "8px 12px")
        .style("box-shadow", "0 2px 10px rgba(0,0,0,0.1)")
        .style("pointer-events", "none")
        .style("font-family", "'Segoe UI', Arial, sans-serif")
        .style("font-size", "13px")
        .style("z-index", "1000")
        .style("max-width", "250px");

    g.selectAll(".link")
        .data(root.links())
        .enter().append("path")
        .attr("class", "link")
        .attr("d", d3.linkVertical()
            .x(d => d.x)
            .y(d => d.y));

    svg.append("defs").selectAll("clipPath")
        .data(root.descendants())
        .enter().append("clipPath")
        .attr("id", d => `clip-${d.data.id}`)
        .append("circle")
        .attr("r", 35);

    const node = g.selectAll(".node")
        .data(root.descendants())
        .enter().append("g")
        .attr("class", d => d.data.id === "org-root" ? "node root-node" : "node")
        .attr("transform", d => `translate(${d.x},${d.y})`)
        .on("click", function (event, d) {
            if (d.data.id === "org-root") return;
            console.log("Navigating to EmployeeID:", d.data.id, "Name:", d.data.Name);

            // Use encodeURIComponent to handle special characters
            const employeeId = encodeURIComponent(d.data.id);
            window.location.href = `ViewEmployee.aspx?empId=${employeeId}`;

            // Prevent any default behavior or propagation
            event.stopPropagation();
            event.preventDefault();
        })
        .style("cursor", d => d.data.id === "org-root" ? "default" : "pointer");

    node.append("circle")
        .attr("r", 40)
        .attr("fill", "none")
        .attr("stroke", "#4a90e2")
        .attr("stroke-width", 3)
        .attr("class", d => {
            const ranking = d.data.Ranking;
            return ranking ? `ranking${ranking}` : "";
        })
        .on("mouseover", function () {
            d3.select(this).attr("stroke-width", 4).attr("stroke", "#2a6fbb");
        })
        .on("mouseout", function () {
            const ranking = d3.select(this).attr("class") || "";
            const strokeWidth = ranking.includes("ranking1") ? 5 :
                ranking.includes("ranking2") ? 4 : 3;
            d3.select(this).attr("stroke-width", strokeWidth);
        });

    node.append("image")
        .attr("xlink:href", d => d.data.ImageUrl || getGravatarUrl(d.data.Email))
        .attr("x", -35)
        .attr("y", -35)
        .attr("width", 70)
        .attr("height", 70)
        .attr("clip-path", d => `url(#clip-${d.data.id})`)
        .attr("class", "node-image")
        .on("mouseover", function (event, d) {
            if (d.data.id === "org-root") return;
            tooltip.transition()
                .duration(200)
                .style("opacity", 1);
            tooltip.html(`
                <div><strong>Employee ID:</strong> ${d.data.id}</div>
                <div><strong>Email:</strong> ${d.data.Email}</div>
                <div><strong>Position:</strong> ${d.data.PositionName}</div>
            `)
                .style("left", (event.pageX + 10) + "px")
                .style("top", (event.pageY - 28) + "px");
        })
        .on("mouseout", function () {
            tooltip.transition()
                .duration(500)
                .style("opacity", 0);
        })
        .on("mousemove", function (event) {
            tooltip
                .style("left", (event.pageX + 10) + "px")
                .style("top", (event.pageY - 28) + "px");
        });

    node.append("text")
        .attr("class", "name")
        .attr("dy", "4em")
        .attr("text-anchor", "middle")
        .text(d => d.data.Name);

    const resetButton = svg.append("g")
        .attr("class", "reset-zoom")
        .attr("transform", `translate(${width - 50}, 20)`)
        .style("cursor", "pointer");

    resetButton.append("rect")
        .attr("width", 30)
        .attr("height", 30)
        .attr("fill", "#f9f9f9")
        .attr("stroke", "#4a90e2")
        .attr("rx", 5)
        .attr("stroke-width", 2);

    resetButton.append("text")
        .attr("x", 15)
        .attr("y", 18)
        .attr("text-anchor", "middle")
        .text("⟲")
        .attr("font-size", "16px")
        .attr("fill", "#4a90e2");

    resetButton.on("click", () => {
        svg.transition()
            .duration(750)
            .call(zoom.transform, d3.zoomIdentity
                .translate(xOffset, yOffset)
                .scale(1));
    });
}