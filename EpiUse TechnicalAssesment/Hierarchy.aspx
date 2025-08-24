<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Hierarchy.aspx.cs" Inherits="EpiUse_TechnicalAssesment.WebForm7" %>

<asp:Content ID="Content1" ContentPlaceHolderID="tab" runat="server">
    Hierarchy
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="headerContentPlaceHolder" runat="server">
    <h1>Employee Management System</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="navContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <div class="container-fluid">
     

        <!-- Search and Filter Panel -->
        <div class="search-panel card">
            <div class="card-header">
                <h2>Search & Filter</h2>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-4">
                        <div class="form-group">
                            <label for="searchName">Name</label>
                            <input type="text" class="form-control" id="searchName" placeholder="Enter name">
                        </div>
                    </div>
                    <div class="col-md-4" style="display: none;">
                        <div class="form-group">
                            <label for="searchPosition">Position</label>
                            <input type="text" class="form-control" id="searchPosition" placeholder="Enter position">
                        </div>
                    </div>
                    <div class="col-md-4 d-flex align-items-end">
                        <button type="button" class="btn btn-primary me-2" onclick="performSearch()">Search</button>
                        <button type="button" class="btn btn-secondary" onclick="clearSearch()">Clear</button>
                    </div>
                </div>
            </div>
        </div>

        <div id="noResultsMessage" class="alert alert-warning" style="display: none;">
            No employees match your search criteria.
        </div>

        <!-- Hierarchy Visualization -->
        <div class="card">
            <div class="card-header">
                <h2>Organization Hierarchy</h2>
            </div>
            <div class="card-body">
                <div id="hierarchy-container" class="hierarchy-container"></div>
            </div>
        </div>
    </div>

    <script src="https://d3js.org/d3.v7.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/crypto-js/4.1.1/crypto-js.min.js"></script>
    <script>
        // Global variables
        let originalHierarchyData = null;
        let currentHierarchyData = null;
        let searchResults = new Set();

        // initialize page
        document.addEventListener("DOMContentLoaded", function () {
            console.log("DOM loaded, initializing page...");

            // Load hierarchy data
            loadHierarchyData();
        });

       
        function loadHierarchyData() {
            if (typeof PageMethods !== 'undefined') {
                PageMethods.GetHierarchyData(
                    function (response) {
                        try {
                            console.log("Raw response:", response);
                            const data = JSON.parse(response);
                            console.log("Parsed data:", data);

                            // Store the original data for search reset
                            originalHierarchyData = data;
                            currentHierarchyData = data;

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
                loadSampleData();
            }
        }

        // Search function
        function performSearch() {
            const nameQuery = document.getElementById('searchName').value.toLowerCase().trim();
            const positionQuery = document.getElementById('searchPosition').value.toLowerCase().trim();

            // Clear previous search results for a new search
            searchResults.clear();

            // If all search fields are empty, show the full hierarchy
            if (!nameQuery && !positionQuery) {
                clearSearch();
                return;
            }

            // Filter the hierarchy based on search criteria
            const filteredData = filterHierarchy(originalHierarchyData, nameQuery, positionQuery);

            // If no matching employees are found, filteredData will be the root node with an empty children array.
            if (filteredData && filteredData.children.length === 0 && (nameQuery || positionQuery)) {
                document.getElementById('noResultsMessage').style.display = 'block';
            } else {
                document.getElementById('noResultsMessage').style.display = 'none';
            }

            currentHierarchyData = filteredData;
            renderD3(currentHierarchyData);
            highlightSearchResults();
        }

        function filterHierarchy(node, nameQuery, positionQuery) {
            if (node.id === "org-root") {
                const filteredChildren = [];
                if (node.children) {
                    for (const child of node.children) {
                        const filteredChild = filterHierarchy(child, nameQuery, positionQuery);
                        if (filteredChild) {
                            filteredChildren.push(filteredChild);
                        }
                    }
                }
                return {
                    ...node,
                    children: filteredChildren
                };
            }

            const matchesSearch = (
                (!nameQuery || (node.Name && node.Name.toLowerCase().includes(nameQuery))) &&
                (!positionQuery || (node.PositionName && node.PositionName.toLowerCase().includes(positionQuery)))
            );

            let filteredChildren = [];
            if (node.children && node.children.length > 0) {
                for (const child of node.children) {
                    const filteredChild = filterHierarchy(child, nameQuery, positionQuery);
                    if (filteredChild) {
                        filteredChildren.push(filteredChild);
                    }
                }
            }
            if (matchesSearch || filteredChildren.length > 0) {
                if (matchesSearch) {
                    searchResults.add(node.id); 
                }

                return {
                    ...node,
                    children: filteredChildren
                };
            }

            return null; 
        }

        function highlightSearchResults() {
            d3.selectAll(".node").each(function (d) {
                const node = d3.select(this);
                if (searchResults.has(d.data.id)) {
                    node.classed("highlighted", true);

                    let current = d;
                    while (current.parent) {
                        d3.selectAll(".link").filter(link =>
                            link.source === current.parent && link.target === current
                        ).classed("highlighted", true);
                        current = current.parent;
                    }
                }
            });
        }

        function clearSearch() {
            document.getElementById('searchName').value = '';
            document.getElementById('searchPosition').value = '';
            document.getElementById('noResultsMessage').style.display = 'none';

            searchResults.clear();
            currentHierarchyData = originalHierarchyData;
            renderD3(originalHierarchyData);
        }

        //  D3 Visualization
        function renderD3(rootData) {
            console.log("Rendering hierarchy with data:", rootData);

            if (!rootData || !rootData.id) {
                console.error("Invalid root data:", rootData);
                alert("Invalid hierarchy data received");
                return;
            }

            const container = document.getElementById("hierarchy-container");
            if (!container) {
                console.error("Container element not found");
                return;
            }

            const width = container.clientWidth;
            const height = container.clientHeight;

            // Clear previous visualization
            d3.select("#hierarchy-container").selectAll("*").remove();

            // Create tree layout - VERTICAL orientation
            const treeLayout = d3.tree()
                .size([width - 100, height - 200])  // Swap for vertical
                .nodeSize([150, 250]);  // Adjust node spacing

            // Create hierarchy
            const root = d3.hierarchy(rootData);
            treeLayout(root);

            // Create SVG
            const svg = d3.select("#hierarchy-container")
                .append("svg")
                .attr("width", width)
                .attr("height", height)
                .attr("viewBox", [0, 0, width, height]);

            //  zoom functionality
            const zoom = d3.zoom()
                .scaleExtent([0.1, 3])
                .on("zoom", function (event) {
                    g.attr("transform", event.transform);
                });

            svg.call(zoom);

            const g = svg.append("g");

            // Calculate bounds for centering
            let x0 = Infinity, x1 = -Infinity, y0 = Infinity, y1 = -Infinity;
            root.each(d => {
                if (d.x > x1) x1 = d.x;
                if (d.x < x0) x0 = d.x;
                if (d.y > y1) y1 = d.y;
                if (d.y < y0) y0 = d.y;
            });

            const dx = x1 - x0;
            const dy = y1 - y0;
            const xOffset = (width - dx) / 2 - x0;
            const yOffset = (height - dy) / 2 - y0;

            g.attr("transform", `translate(${xOffset},${yOffset})`);

            // Create tooltip
            let tooltip;
            try {
                tooltip = d3.select("body").append("div")
                    .attr("class", "tooltip")
                    .style("opacity", 0)
                    .style("position", "absolute");
                console.log("Tooltip created successfully");
            } catch (error) {
                console.error("Error creating tooltip:", error);
            }

            
            g.selectAll(".link")
                .data(root.links())
                .enter().append("path")
                .attr("class", "link")
                .attr("d", d3.linkVertical()  
                    .x(d => d.x)
                    .y(d => d.y));

            // Create clipPath for circular images
            const defs = svg.append("defs");
            root.descendants().forEach(d => {
                defs.append("clipPath")
                    .attr("id", `clip-${d.data.id}`)
                    .append("circle")
                    .attr("r", 35);
            });

            // Draw nodes with click handler for navigation
            const node = g.selectAll(".node")
                .data(root.descendants())
                .enter().append("g")
                .attr("class", d => d.data.id === "org-root" ? "node root-node" : "node")
                .attr("transform", d => `translate(${d.x},${d.y})`)
                .on("click", function (event, d) {
                    // Prevent navigation on the root node
                    if (d.data.id === "org-root") return;

                    // Prevent event propagation to avoid conflicts
                    event.stopPropagation();

                    // Get the employee ID - try multiple possible properties
                    const employeeId = d.data.id || d.data.EmployeeID || d.data.employeeId;

                    if (!employeeId || employeeId === "org-root") {
                        console.error("No valid employee ID found for this node:", d.data);
                        return;
                    }

                    // Navigate to the employee's page
                    window.location.href = `ViewEmployee.aspx?EmployeeID=${encodeURIComponent(employeeId)}`;
                })
                .style("cursor", d => d.data.id === "org-root" ? "default" : "pointer");

            node.append("title")
                .text(d => d.data.id === "org-root" ? "Organization" :
                    `${d.data.Name}\n${d.data.PositionName}\n${d.data.Email}`);

            node.append("circle")
                .attr("r", 40)
                .attr("fill", "none")
                .attr("stroke", "#4a90e2")
                .attr("stroke-width", 3)
                .attr("class", d => {
                    return "";
                });

            node.append("image")
                .attr("xlink:href", d => d.data.ImageUrl || getGravatarUrl(d.data.Email))
                .attr("x", -35)
                .attr("y", -35)
                .attr("width", 70)
                .attr("height", 70)
                .attr("clip-path", d => `url(#clip-${d.data.id})`)
                .attr("class", "node-image");

            // Employee full name
            node.append("text")
                .attr("class", "name")
                .attr("dy", "3.5em") 
                .attr("text-anchor", "middle")
                .text(d => d.data.Name);

            // Position text
            node.append("text")
                .attr("class", "title")
                .attr("dy", "5em") 
                .attr("text-anchor", "middle")
                .text(d => d.data.PositionName || "");

            // Reset zoom button
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

            console.log("Hierarchy rendered successfully");
        }

        // Gravatar functionality 
        function getGravatarUrl(email) {
            if (!email) return "https://www.gravatar.com/avatar/?d=identicon";

            try {
                const emailHash = CryptoJS.MD5(email.trim().toLowerCase()).toString();
                return `https://www.gravatar.com/avatar/${emailHash}?d=identicon`;
            } catch (error) {
                console.error("Error generating Gravatar URL:", error);
                return "https://www.gravatar.com/avatar/?d=identicon";
            }
        }
    </script>
</asp:Content>
