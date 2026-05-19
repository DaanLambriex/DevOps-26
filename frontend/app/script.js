document.getElementById("loadRepos").addEventListener("click", async () => {
    
    const response = await fetch("http://localhost:5000/api/repositories");
    const repos = await response.json();

    const repoList = document.getElementById("repoList");
    repoList.innerHTML = "";

    repos.forEach(repo => {
        const li = document.createElement("li");

        const visibilityBadge = document.createElement("span");
        visibilityBadge.textContent = repo.isPrivate ? "[Private]" : "[Public]";
        visibilityBadge.className = repo.isPrivate ? "badge private" : "badge public";

        const repoText = document.createElement("span");
        repoText.textContent = ` ${repo.name} - ${repo.description ?? "Geen beschrijving"} `;

        const repoLink = document.createElement("a");
        repoLink.href = repo.htmlUrl;
        repoLink.textContent = "Open op GitHub";
        repoLink.target = "_blank";

        li.appendChild(visibilityBadge);
        li.appendChild(repoText);
        const separator = document.createTextNode(" - ");
        li.appendChild(separator);
        li.appendChild(repoLink);

        repoList.appendChild(li);

    });

});