document.getElementById("loadRepos").addEventListener("click", async () => {
    const repoList = document.getElementById("repoList");
    const button = document.getElementById("loadRepos");

    repoList.innerHTML = "<li>Repositories laden...</li>";
    button.disabled = true;
    button.textContent = "Laden...";
    
    try {
        const response = await fetch("https://githubmanager-daan-grang0c8b5gjdadg.westeurope-01.azurewebsites.net/api/repositories");

        if (!response.ok) {
            throw new Error("Backend gaf een foutmelding");
        }

        const repos = await response.json();
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
    } catch (error) {
        repoList.innerHTML = "<li>Er ging iets mis bij het laden van de repositories.</li>";
        console.error(error);
    } finally {
        button.disabled = false;
        button.textContent = "Load Repos";

    });

});