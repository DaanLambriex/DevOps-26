let allRepos = [];
let selectedTopics = [];

const API_BASE_URL = "https://githubmanager-daan-grang0c8b5gjdadg.westeurope-01.azurewebsites.net";

document.getElementById("loadRepos").addEventListener("click", loadRepos);

async function loadRepos() {
    const activeRepoList = document.getElementById("activeRepoList");
    const archivedRepoList = document.getElementById("archivedRepoList");
    const button = document.getElementById("loadRepos");

    activeRepoList.innerHTML = "<li>Repositories laden...</li>";
    archivedRepoList.innerHTML = "<li>Repositories laden...</li>";
    button.disabled = true;
    button.textContent = "Laden...";

    try {
        const response = await fetch(`${API_BASE_URL}/api/repositories`);

        if (!response.ok) {
            throw new Error("Backend gaf een foutmelding");
        }

        const repos = await response.json();

        allRepos = repos;
        fillTopicFilters(repos);
        displayRepos(repos);

    } catch (error) {
        activeRepoList.innerHTML = "<li>Er ging iets mis bij het laden van de repositories.</li>";
        archivedRepoList.innerHTML = "";
        console.error(error);
    } finally {
        button.disabled = false;
        button.textContent = "Load Repos";
    }
}

function displayRepos(repos) {
    const activeRepoList = document.getElementById("activeRepoList");
    const archivedRepoList = document.getElementById("archivedRepoList");

    activeRepoList.innerHTML = "";
    archivedRepoList.innerHTML = "";

    const filteredRepos = repos.filter(repo => {
        if (selectedTopics.length === 0) {
            return true;
        }

        return selectedTopics.every(topic =>
            repo.topics && repo.topics.includes(topic)
        );
    });

    filteredRepos.forEach(repo => {
        const li = document.createElement("li");

        const visibilityBadge = document.createElement("span");
        visibilityBadge.textContent = repo.isPrivate ? "[Private]" : "[Public]";
        visibilityBadge.className = repo.isPrivate ? "badge private" : "badge public";

        const repoText = document.createElement("span");
        repoText.textContent = repo.name + " - " + (repo.description ?? "Geen beschrijving") + " ";

        const repoLink = document.createElement("a");
        repoLink.href = repo.htmlUrl;
        repoLink.textContent = "Open op GitHub";
        repoLink.target = "_blank";

        li.appendChild(visibilityBadge);
        li.appendChild(repoText);
        li.appendChild(document.createTextNode(" - "));
        li.appendChild(repoLink);

        if (repo.topics && repo.topics.length > 0) {
            const topicsDiv = document.createElement("div");
            topicsDiv.className = "topics";

            repo.topics.forEach(topic => {
                const badge = document.createElement("span");
                badge.className = "topic-badge";
                badge.textContent = topic;

                topicsDiv.appendChild(badge);
            });

            li.appendChild(document.createElement("br"));
            li.appendChild(topicsDiv);
        }

        const archiveButton = document.createElement("button");
        archiveButton.textContent = repo.archived ? "Herstel" : "Archiveer";

        archiveButton.addEventListener("click", async () => {
            await fetch(`${API_BASE_URL}/api/repositories/${repo.name}/archive`, {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    archived: !repo.archived
                })
            });

            await loadRepos();
        });

        li.appendChild(document.createTextNode(" - "));
        li.appendChild(archiveButton);

        if (repo.archived) {
            archivedRepoList.appendChild(li);
        } else {
            activeRepoList.appendChild(li);
        }
    });
}

function fillTopicFilters(repos) {
    const topicFilters = document.getElementById("topicFilters");
    const topics = new Set();

    repos.forEach(repo => {
        if (repo.topics) {
            repo.topics.forEach(topic => topics.add(topic));
        }
    });

    topicFilters.innerHTML = "";

    topics.forEach(topic => {
        const label = document.createElement("label");
        label.className = "topic-filter";

        const checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.value = topic;

        checkbox.addEventListener("change", () => {
            selectedTopics = Array.from(
                document.querySelectorAll("#topicFilters input:checked")
            ).map(cb => cb.value);

            displayRepos(allRepos);
        });

        label.appendChild(checkbox);
        label.appendChild(document.createTextNode(" " + topic));

        topicFilters.appendChild(label);
    });
}

document.getElementById("clearFilters").addEventListener("click", () => {
    document.querySelectorAll("#topicFilters input")
        .forEach(checkbox => checkbox.checked = false);

    selectedTopics = [];
    displayRepos(allRepos);
});