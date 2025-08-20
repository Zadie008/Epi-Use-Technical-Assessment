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
    <h2>Company Hierarchy</h2>
    <div id="hierarchy-container" style="width: 100%; height: 800px; border: 1px solid #e0e0e0;"></div>

   <script>
       // Global variables
       let originalHierarchyData = null;
       let currentHierarchyData = null;
       let searchResults = new Set();

       // ========== Helper for Gravatar ==========
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

       // ========== Search Functions ==========
       function performSearch() {
           const nameQuery = document.getElementById('searchName').value.toLowerCase().trim();
           const positionQuery = document.getElementById('searchPosition').value.toLowerCase().trim();
           const locationQuery = document.getElementById('searchLocation').value.toLowerCase().trim();

           // Clear previous search results for a new search
           searchResults.clear();

           // If all search fields are empty, show the full hierarchy
           if (!nameQuery && !positionQuery && !locationQuery) {
               clearSearch();
               return;
           }

           // Filter the hierarchy based on search criteria
           const filteredData = filterHierarchy(originalHierarchyData, nameQuery, positionQuery, locationQuery);

           // If no matching employees are found, filteredData will be the root node with an empty children array.
           if (filteredData && filteredData.children.length === 0 && (nameQuery || positionQuery || locationQuery)) {
               document.getElementById('noResultsMessage').style.display = 'block';
           } else {
               document.getElementById('noResultsMessage').style.display = 'none';
           }

           currentHierarchyData = filteredData;
           renderD3(currentHierarchyData);
           highlightSearchResults();
       }

       function filterHierarchy(node, nameQuery, positionQuery, locationQuery) {
           // We always want to return a copy of the root node to maintain the structure.
           // The filtering logic only applies to its children.
           if (node.id === "org-root") {
               const filteredChildren = [];
               if (node.children) {
                   for (const child of node.children) {
                       const filteredChild = filterHierarchy(child, nameQuery, positionQuery, locationQuery);
                       if (filteredChild) {
                           filteredChildren.push(filteredChild);
                       }
                   }
               }

               // Return the root node with the filtered list of children.
               // It's important to return this even if the list is empty.
               return {
                   ...node,
                   children: filteredChildren
               };
           }

           // This section handles the rest of the nodes.
           const matchesSearch = (
               (!nameQuery || (node.Name && node.Name.toLowerCase().includes(nameQuery))) &&
               (!positionQuery || (node.PositionName && node.PositionName.toLowerCase().includes(positionQuery))) &&
               (!locationQuery || (node.Location && node.Location.toLowerCase().includes(locationQuery)))
           );

           let filteredChildren = [];
           if (node.children && node.children.length > 0) {
               for (const child of node.children) {
                   const filteredChild = filterHierarchy(child, nameQuery, positionQuery, locationQuery);
                   if (filteredChild) {
                       filteredChildren.push(filteredChild);
                   }
               }
           }

           // A node is included if it matches the search criteria or if any of its children match.
           if (matchesSearch || filteredChildren.length > 0) {
               if (matchesSearch) {
                   searchResults.add(node.id); // Add to set for highlighting
               }

               return {
                   ...node,
                   children: filteredChildren
               };
           }

           return null; // Return null if the node and its descendants do not match
       }

       function highlightSearchResults() {
           d3.selectAll(".node").each(function (d) {
               const node = d3.select(this);
               if (searchResults.has(d.data.id)) {
                   node.classed("highlighted", true);

                   // Also highlight the path to this node
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
           document.getElementById('searchLocation').value = '';
           document.getElementById('noResultsMessage').style.display = 'none';

           searchResults.clear();
           currentHierarchyData = originalHierarchyData;
           renderD3(originalHierarchyData);
       }

       // ========== Main D3 Hierarchy ==========
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

           // Add zoom functionality
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

           // Create tooltip FIRST
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

           // Draw links - VERTICAL orientation
           g.selectAll(".link")
               .data(root.links())
               .enter().append("path")
               .attr("class", "link")
               .attr("d", d3.linkVertical()  // Changed to vertical
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

           // Draw nodes
           const node = g.selectAll(".node")
               .data(root.descendants())
               .enter().append("g")
               .attr("class", d => d.data.id === "org-root" ? "node root-node" : "node")
               .attr("transform", d => `translate(${d.x},${d.y})`)  // Swap x and y for vertical
               .on("click", function (event, d) {
                   if (d.data.id === "org-root") return;
                   console.log("Clicked on:", d.data.id);
                   // In a real implementation, this would navigate to the employee page
                   alert(`Would navigate to employee: ${d.data.Name}`);
                   event.preventDefault();
               })
               .style("cursor", d => d.data.id === "org-root" ? "default" : "pointer");

           // Add browser tooltip as fallback
           node.append("title")
               .text(d => d.data.id === "org-root" ? "Organization" :
                   `${d.data.Name}\n${d.data.PositionName}\n${d.data.Email}`);

           // Node circles (border only)
           node.append("circle")
               .attr("r", 40)
               .attr("fill", "none")
               .attr("stroke", "#4a90e2")
               .attr("stroke-width", 3)
               .attr("class", d => {
                   // Add your ranking classes if available in data
                   return "";
               })
               .on("mouseover", function (event, d) {
                   if (d.data.id === "org-root") return;
                   if (!tooltip) return;

                   console.log("Mouseover event triggered for:", d.data);

                   // Check if data exists before accessing it
                   const empId = d.data.id || d.data.EmployeeID || "N/A";
                   const email = d.data.Email || "N/A";
                   const position = d.data.PositionName || d.data.Position || "N/A";
                   const location = d.data.Location || d.data.Office || "N/A";

                   console.log("Tooltip content:", { empId, email, position, location });

                //   // Show tooltip
                //   tooltip.transition()
                //       .duration(200)
                //       .style("opacity", 1);

                //   tooltip.html(`
                //    <div><strong>Employee ID:</strong> ${empId}</div>
                //    <div><strong>Email:</strong> ${email}</div>
                //    <div><strong>Position:</strong> ${position}</div>
                //    <div><strong>Location:</strong> ${location}</div>
                //`)
                //       .style("left", (event.pageX + 10) + "px")
                //       .style("top", (event.pageY - 28) + "px");

                //   console.log("Tooltip positioned at:", event.pageX + 10, event.pageY - 28);
               })
               //.on("mouseout", function () {
               //    if (tooltip) {
               //        tooltip.transition()
               //            .duration(500)
               //            .style("opacity", 0);
               //    }
               //})
               //.on("mousemove", function (event) {
               //    if (tooltip) {
               //        tooltip
               //            .style("left", (event.pageX + 10) + "px")
               //            .style("top", (event.pageY - 28) + "px");
               //    }
               //});

           // Add images to nodes (Gravatar or database images)
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
               .attr("dy", "3.5em")  // Position below image
               .attr("text-anchor", "middle")
               .text(d => d.data.Name);

           // Position text
           node.append("text")
               .attr("class", "title")
               .attr("dy", "5em")  // Position below name
               .attr("text-anchor", "middle")
               .text(d => d.data.PositionName || "");

           //reset zoom button
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

       // ========== Page Load ==========
       document.addEventListener("DOMContentLoaded", function () {
           console.log("DOM loaded, calling PageMethods...");

           if (typeof PageMethods !== 'undefined') {
               PageMethods.GetHierarchyData(
                   function (response) {
                       try {
                           console.log("Raw response:", response);
                           const data = JSON.parse(response);
                           console.log("Parsed data:", data);

                           // Store the original data for search reset
                           originalHierarchyData = data;

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
   </script>
</asp:Content>
