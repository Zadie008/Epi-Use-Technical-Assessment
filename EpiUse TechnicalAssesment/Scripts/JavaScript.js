// ========== Helper for Gravatar ==========
function md5(string) {
    return CryptoJS.MD5(string).toString();
}

// ========== Main D3 Hierarchy ==========
function renderD3(rootData) {
    console.log("Rendering hierarchy with data:", rootData);

    const container = document.getElementById("hierarchy-container");
    if (!container) {
        console.error("Container element not found");
        return;
    }

    const width = container.clientWidth;
    const height = container.clientHeight;

    // Clear previous visualization
    d3.select("#hierarchy-container").selectAll("*").remove();

    // Create tree layout
    const treeLayout = d3.tree()
        .size([height - 100, width - 200])
        .nodeSize([150, 250]);

    // Create hierarchy
    const root = d3.hierarchy(rootData);
    treeLayout(root);

    // Create SVG
    const svg = d3.select("#hierarchy-container")
        .append("svg")
        .attr("width", width)
        .attr("height", height);

    const g = svg.append("g");

    // Links
    g.selectAll(".link")
        .data(root.links())
        .enter().append("path")
        .attr("class", "link")
        .attr("d", d3.linkHorizontal()
            .x(d => d.y)
            .y(d => d.x));

    // Nodes
    const node = g.selectAll(".node")
        .data(root.descendants())
        .enter().append("g")
        .attr("class", "node")
        .attr("transform", d => `translate(${d.y},${d.x})`);

    // Node circles
    node.append("circle")
        .attr("r", 45)
        .attr("fill", "#fff")
        .attr("stroke", "#999")
        .attr("stroke-width", 2);

    // Name text
    node.append("text")
        .attr("dy", "0.31em")
        .attr("text-anchor", "middle")
        .text(d => d.data.Name.split(" ")[0]) // First name
        .attr("font-weight", "bold");

    node.append("text")
        .attr("dy", "1.5em")
        .attr("text-anchor", "middle")
        .text(d => d.data.Name.split(" ")[1]); // Last name

    // Position text
    node.append("text")
        .attr("dy", "2.7em")
        .attr("text-anchor", "middle")
        .text(d => d.data.Position)
        .attr("font-size", "0.8em")
        .attr("fill", "#555");

    // Center the view
    const bounds = root.descendants().reduce((acc, d) => ({
        x: [Math.min(acc.x[0], d.x), Math.max(acc.x[1], d.x)],
        y: [Math.min(acc.y[0], d.y), Math.max(acc.y[1], d.y)]
    }), { x: [0, 0], y: [0, 0] });

    const dx = bounds.x[1] - bounds.x[0];
    const dy = bounds.y[1] - bounds.y[0];
    const scale = Math.min(0.9 * width / dx, 0.9 * height / dy);

    g.attr("transform", `translate(${(width - dx * scale) / 2},${(height - dy * scale) / 2})scale(${scale})`);
}

// ========== Page Load ==========
document.addEventListener("DOMContentLoaded", function () {
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