document.getElementById("loadRepos").addEventListener("click", async () => {

    const response = await fetch("https://localhost:7005/api/repositories");

    const repos = await response.json();

    const repoList = document.getElementById("repoList");

    repoList.innerHTML = "";

    repos.forEach(repo => {

        const li = document.createElement("li");

        li.textContent = repo.name + " - " + repo.description;

        repoList.appendChild(li);

    });

});