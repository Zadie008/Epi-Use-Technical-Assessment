// ========== Helper for Gravatar ==========
function md5(string) {
    return CryptoJS.MD5(string).toString();
}

// ========== Main D3 Hierarchy ==========
var zoom;

function renderD3(rootData) {
    const container = document.getElementById("hierarchy-container");
    const width = container.clientWidth;
    const height = container.clientHeight;

    d3.select("#hierarchy-container").selectAll("*").remove();

    const treeLayout = d3.tree()
        .size([height - 100, width - 200])
        .nodeSize([120, 200])
        .separation((a, b) => (a.parent === b.parent ? 1 : 2));

    const root = d3.hierarchy(rootData);
    treeLayout(root);

    const svg = d3.select("#hierarchy-container")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .call(zoom = d3.zoom()
            .scaleExtent([0.3, 3])
            .on("zoom", zoomed));

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

    // Make nodes clickable
    node.on("click", function (event, d) {
        // Check both possible property names for maximum compatibility
        var empId = d.data.id || d.data.EmployeeNumber;
        if (empId) {
            window.location.href = "ViewEmployee.aspx?empId=" + encodeURIComponent(empId);
        } else {
            console.error("No employee ID found in node data:", d.data);
        }
    });

    // Clip path for circular images
    svg.append("defs").append("clipPath")
        .attr("id", "circle-clip")
        .append("circle")
        .attr("r", 35)
        .attr("cx", 0)
        .attr("cy", 0);

    // Profile image (Base64 or Gravatar fallback)
    node.append("image")
        .attr("xlink:href", d => {
            if (d.data.Photo && d.data.Photo.trim() !== "") {
                return "data:image/png;base64," + d.data.Photo;
            } else {
                const email = d.data.Email ? d.data.Email.trim().toLowerCase() : "";
                const hash = md5(email);
                return `https://www.gravatar.com/avatar/${hash}?s=70&d=identicon`;
            }
        })
        .attr("x", -35)
        .attr("y", -35)
        .attr("width", 70)
        .attr("height", 70)
        .attr("clip-path", "url(#circle-clip)");

    // Name
    node.append("text")
        .attr("dy", "3.5em")
        .attr("class", "name")
        .style("text-anchor", "middle")
        .style("font-weight", "600")
        .text(d => d.data.Name);

    // Role
    node.append("text")
        .attr("dy", "5.2em")
        .attr("class", "title")
        .style("text-anchor", "middle")
        .text(d => d.data.Role);

    // Tooltip
    const tooltip = d3.select("body").append("div")
        .attr("class", "tooltip")
        .style("opacity", 0);

    node.on("mouseover", function (event, d) {
        tooltip.transition().duration(200).style("opacity", .9);
        tooltip.html(`
            <strong>${d.data.Name}</strong><br/>
            ${d.data.Role}<br/>
            ${d.data.Department} - ${d.data.Location}<br/>
            ${d.data.Email}
        `)
            .style("left", (event.pageX + 10) + "px")
            .style("top", (event.pageY - 28) + "px");
    })
        .on("mouseout", function () {
            tooltip.transition().duration(500).style("opacity", 0);
        });

    // Center and zoom to fit
    centerAndZoom(svg, g, root, width, height);
}

// Zoom handler
function zoomed(event) {
    d3.select("#hierarchy-container g").attr("transform", event.transform);
}

// Center tree on load
function centerAndZoom(svg, g, root, width, height) {
    const nodes = root.descendants();
    const xExtent = d3.extent(nodes, d => d.x);
    const yExtent = d3.extent(nodes, d => d.y);

    const treeWidth = yExtent[1] - yExtent[0];
    const treeHeight = xExtent[1] - xExtent[0];

    const scale = 0.9 / Math.max(treeWidth / width, treeHeight / height);

    const translateX = (width - treeWidth * scale) / 2 - yExtent[0] * scale;
    const translateY = (height - treeHeight * scale) / 2 - xExtent[0] * scale + 50;

    svg.transition()
        .duration(500)
        .call(zoom.transform, d3.zoomIdentity
            .translate(translateX, translateY)
            .scale(scale));
}

// Handle resize
let resizeTimer;
window.addEventListener('resize', function () {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function () {
        const currentData = window.currentHierarchyData;
        if (currentData) {
            renderD3(currentData);
        }
    }, 250);
});

// ========== Build Hierarchy from Flat Data ==========
function buildHierarchy(data) {
    if (!data || !Array.isArray(data)) {
        console.error("Invalid data format:", data);
        return;
    }

    const map = {};
    data.forEach(emp => {
        map[emp.EmployeeNumber] = { ...emp, children: [] };
    });

    let rootNodes = [];
    data.forEach(emp => {
        if (emp.ManagerID && map[emp.ManagerID]) {
            map[emp.ManagerID].children.push(map[emp.EmployeeNumber]);
        } else {
            rootNodes.push(map[emp.EmployeeNumber]);
        }
    });

    let rootData;
    if (rootNodes.length > 1) {
        rootData = {
            Name: "Organization",
            Role: "",
            children: rootNodes
        };
    } else {
        rootData = rootNodes[0] || data[0];
    }

    renderD3(rootData);
}

// ========== Page Load ==========
document.addEventListener("DOMContentLoaded", function () {
    PageMethods.GetHierarchyData(
        function (data) {
            try {
                window.currentHierarchyData = data;
                buildHierarchy(data);
            } catch (e) {
                console.error("Error:", e);
                alert("Error loading hierarchy. Check console for details.");
            }
        },
        function (error) {
            console.error("AJAX error:", error);
            alert("Failed to load data from server.");
        }
    );
});
